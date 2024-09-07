using DiscordBot.Application;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.DiscordAPI.Structures.Payloads;
using DiscordBot.DiscordAPI.Structures.Voice;
using DiscordBot.Logging;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Timers;
using WebSocket4Net;
using Timer = System.Timers.Timer;

namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// Handles communication on the Discord Gateway websocket
    /// </summary>
    internal class DiscordGatewayClient
    {
        /// <summary>
        /// Default Heartbeat Interval to use based on previous session values
        /// </summary>
        private const int DEFAULT_HEARTBEAT_INTERVAL = 41250;

        public delegate void OnMessageCallback(DiscordGatewayEvent payload);

        private readonly WebSocket _webSocket;

        private OnMessageCallback? _messageReceivedCallback;
        private OnMessageCallback? readyCallback; //TODO: Remove these, handle ready event elsewhere
        private OnMessageCallback? guildCreateCallback;

        private readonly string _token;
        private readonly string _gatewayUri;
        private readonly int _gatewayIntents;

        private Timer? _heartbeatTimer;
        private int _heartbeatInterval;
        private int? _lastSequenceNumber;
        private bool _heartbeatAcknowledged;

        private readonly ILogger _logger;

        public DiscordGatewayClient(IOptions<BotSecretsConfiguration> authentication, IOptions<DiscordGatewayConfiguration> gatewayConfig, 
            ApplicationLifetimeService applicationLifetime, ILogger logger)
        {
            _gatewayUri = gatewayConfig.Value.WebSocket;
            _webSocket = new WebSocket(_gatewayUri);
            _token = authentication.Value.Token;
            _webSocket.Opened += OnSocketOpened;
            _webSocket.Closed += OnSocketClosed;
            _webSocket.MessageReceived += OnEventReceived;
            _webSocket.Error += OnError;
            //webSocket.DataReceived //TODO: Can we receive this event from Discord?


            // Get Gateway Intents or use defaults
            _gatewayIntents = gatewayConfig.Value.GatewayIntents ??
                (int)(GatewayIntents.Guilds
                | GatewayIntents.GuildMembers
                | GatewayIntents.GuildVoiceStates
                | GatewayIntents.GuildPresences // Required for user list
                | GatewayIntents.GuildMessages
                | GatewayIntents.DirectMessages
                | GatewayIntents.MessageContent
                );

            _logger = logger;

            //applicationLifetime.RegisterShuttingDownCallback(async() => { await DisconnectWebsocket(); });
        }

        public async Task ConnectWebsocket()
        {
            await _webSocket.OpenAsync();
        }

        public async Task DisconnectWebsocket()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync();
            }
        }

        public void OnSocketOpened(object? sender, EventArgs e)
        {
            //TODO: 
        }

        public void OnSocketClosed(object? sender, EventArgs e)
        {
            //heartbeatTimer?.Stop();

            _logger.LogInfo("Websocket closed: " + e.ToString());
        }

        public void OnEventReceived(object? sender, MessageReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Message))
            {     
                DiscordGatewayEvent gatewayEvent = JsonConvert.DeserializeObject<DiscordGatewayEvent>(e.Message)!;
                if (gatewayEvent != null)
                {
                    // Update the last sequence received, used when sending heartbeats
                    _lastSequenceNumber = gatewayEvent.Sequence.GetValueOrDefault();

                    HandleOpcode(gatewayEvent);

                    _logger.LogDebug("Received Event:\n" + gatewayEvent.ToJson(Formatting.Indented));
                }
            }
            else
            {
                _logger.LogWarning("DiscordGatewayClient received empty Gateway Event");
            }
        }

        private void OnMessageReceived(DiscordGatewayEvent gatewayEvent)
        {
            _messageReceivedCallback?.Invoke(gatewayEvent);
        }

        public void OnError(object? sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            _logger.LogException(e.Exception);
        }

        public void SendMessage(string message)
        {
            _webSocket.Send(message);
        }

        public void RegisterMessageReceivedCallback(OnMessageCallback callback)
        {
            _messageReceivedCallback += callback;
        }

        public void RegisterReadyCallback(OnMessageCallback callback)
        {
            readyCallback += callback;
        }

        public void RegisterGuildCreateCallback(OnMessageCallback callback)
        {
            guildCreateCallback += callback;
        }

        /// <summary>
        /// Handles gateway events based on opcode
        /// Note that only opcodes marked as "receive" are handled here<br/>
        /// <see href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-opcodes"/>
        /// </summary>
        /// <param name="gatewayEvent">Gateway event received from Discord gateway</param>
        public void HandleOpcode(DiscordGatewayEvent gatewayEvent)
        {
            switch (gatewayEvent.GatewayOpcode)
            {
                case DiscordGatewayOpcode.Dispatch:
                    //MessageReceived listeners should handle standard messages
                    OnMessageReceived(gatewayEvent);
                    break;
                case DiscordGatewayOpcode.Heartbeat: //Heartbeat is marked as send/receive
                    _logger.LogInfo("Received Heartbeat\n" + gatewayEvent.ToJson(Formatting.Indented));
                    break;
                case DiscordGatewayOpcode.Reconnect:
                    //TODO: Attempt to reconnect
                    //This is expected if the bot has been live for a while
                    break;
                case DiscordGatewayOpcode.InvalidSession:
                    //TODO: Attempt to reconnect and identify again as needed
                    break;
                case DiscordGatewayOpcode.Hello:
                    var hello = gatewayEvent.EventData?.ToObject<DiscordHelloPayload>();
                    if (hello != null) { HandleGatewayHello(hello); }
                    break;
                case DiscordGatewayOpcode.HeartbeatAcknowledge:
                    _heartbeatAcknowledged = true;
                    break;
                default:
                    _logger.LogInfo($"HandleOpcode method received unexpected Opcode:\n{gatewayEvent.ToJson()}");
                    break;
            }
        }

        public void HandleGatewayHello(DiscordHelloPayload payload)
        {
            StartHeartbeatTimer(payload.HeartbeatInterval);

            //Respond with Identify payload
            SendIdentifyPayload();
        }

        public void SendIdentifyPayload()
        {
            DiscordGatewayEvent gatewayEvent = new DiscordGatewayEvent(DiscordGatewayOpcode.Identify,
                new DiscordIdentifyPayload(_token,
                new() { Device = ".Net" },
                _gatewayIntents));

            _webSocket.Send(gatewayEvent.ToJson());
        }

        public void StartHeartbeatTimer(int interval)
        {
            _heartbeatTimer?.Stop();

            _heartbeatInterval = interval;
            _heartbeatTimer = new Timer(_heartbeatInterval);
            _heartbeatTimer.Elapsed += SendHeartbeat;
            _heartbeatTimer.Start();
            _heartbeatAcknowledged = true;
        }

        private void SendHeartbeat(object? sender, ElapsedEventArgs e)
        {
            if (_heartbeatAcknowledged)
            {
                DiscordHeartbeatPayload heartbeat = new(_lastSequenceNumber);
                SendMessage(heartbeat.ToJson());
                _heartbeatAcknowledged = false;
            }
            else
            {
                //TODO: Handle missed heartbeats
            }
        }

        private void StopHeartbeatTimer()
        {
            _heartbeatTimer?.Stop();
        }

        public void SendVoiceUpdate(DiscordVoiceStateStructure voiceState)
        {
            _logger.LogInfo("Attempting to join/leave voice channel: " + voiceState.ChannelID);
            DiscordGatewayEvent voiceEvent = new DiscordGatewayEvent(DiscordGatewayOpcode.VoiceStateUpdate, voiceState);

            _webSocket.Send(voiceEvent.ToJson());
        }
    }
}
