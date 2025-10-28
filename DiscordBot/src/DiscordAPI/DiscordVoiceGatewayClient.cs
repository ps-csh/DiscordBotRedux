using DiscordBot.Controller;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.DiscordAPI.Structures.Payloads;
using DiscordBot.DiscordAPI.Structures.Voice;
using DiscordBot.Logging;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WebSocket4Net;
using Timer = System.Timers.Timer;
using Concentus.Common;
using Concentus.Structs;
using Concentus.Enums;

namespace DiscordBot.DiscordAPI
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/topics/voice-connections"/>
    /// </remarks>
    internal class DiscordVoiceGatewayClient
    {
        /// <summary>
        /// Gateway version, range 1-4.
        /// Discord recommends the most up-to-date version.
        /// Append ?v=4 to the gateway url
        /// //TODO: Move to appsettings
        /// </summary>
        private const int VOICE_GATEWAY_VERSION = 4;

        private readonly string _botID;
        private WebSocket? _voiceWebSocket;
        private UdpClient? _voiceUDPClient;

        private string? _endpoint;
        private string? _sessionID;
        private string? _channelID;
        private string? _guildID;
        private string? _token;
        private string? _udpIpAddress;
        private int _udpPort;

        private bool _waitingForGatewayDetails;
        private bool _voiceStateUpdateReceived;
        private bool _voiceServerUpdateReceived;

        private Timer? _heartbeatTimer;
        private int _heartbeatInterval;
        private long? _lastHeartbeatNonce;
        private bool _heartbeatAcknowledged;

        private readonly DiscordGatewayClient _gatewayClient;
        private readonly ILogger _logger;
        private ConsoleIOHandler _ioHandler; //TEMP

        public DiscordVoiceGatewayClient(IOptions<BotSettingsConfiguration> botSettings, 
            DiscordGatewayClient gatewayClient, ILogger logger, ConsoleIOHandler consoleIOHandler)
        {
            _gatewayClient = gatewayClient;
            _gatewayClient.RegisterMessageReceivedCallback(OnGatewayEventReceived);
            _logger = logger;
            _botID = botSettings.Value.BotID;
            _ioHandler = consoleIOHandler;
        }

        public void PrepareForGatewayConnection(string? expectedGuildID)
        {
            _waitingForGatewayDetails = true;
            _guildID = expectedGuildID;
            _logger.LogInfo("Voice Gateway is awaiting gateway details");
        }

        public void CloseGateway()
        {
            try
            {
                //Reset states
                _waitingForGatewayDetails = _voiceStateUpdateReceived = _voiceServerUpdateReceived = false;
                _endpoint = _udpIpAddress = _channelID = _guildID = null;
                _udpPort = 0;
                _heartbeatTimer?.Stop();
                _voiceUDPClient?.Close();
                _voiceWebSocket?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        public void OnGatewayEventReceived(DiscordGatewayEvent gatewayEvent)
        {
            if (_waitingForGatewayDetails)
            {
                if (gatewayEvent.EventType == DiscordGatewayEventTypes.VOICE_STATE_UPDATE)
                {
                    _logger.LogInfo($"Received VOICE_STATE_UPDATE in VoiceGatewayClient");
                    var voiceStateUpdate = gatewayEvent.EventData?.ToObject<DiscordVoiceStateStructure>();
                    if (voiceStateUpdate != null)
                    {
                        if (voiceStateUpdate.UserID == _botID)
                        {
                            _sessionID = voiceStateUpdate.SessionID;
                            _channelID = voiceStateUpdate.ChannelID;
                            _voiceStateUpdateReceived = true;
                            ConnectToGateway();
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Could not convert Gateway event Payload to DiscordVoiceStateStructure");
                    }
                }
                else if (gatewayEvent.EventType == DiscordGatewayEventTypes.VOICE_SERVER_UPDATE)
                {
                    _logger.LogInfo($"Received VOICE_SERVER_UPDATE in VoiceGatewayClient");
                    var voiceServerUpdate = gatewayEvent.EventData?.ToObject<DiscordVoiceServerUpdateStructure>();
                    if (voiceServerUpdate != null)
                    {
                        if (voiceServerUpdate.GuildID == _guildID)
                        {
                            _token = voiceServerUpdate.Token;
                            _endpoint = $"wss://{voiceServerUpdate.Endpoint}/?v={VOICE_GATEWAY_VERSION}";
                            //_endpoint = $"wss://{voiceServerUpdate.Endpoint}";
                            _voiceServerUpdateReceived = true;
                            ConnectToGateway();
                        }
                        else
                        {
                            _logger.LogWarning("VOICE_SERVER_UPDATE had different guild ID than expected");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Could not convert Gateway event Payload to DiscordVoiceServerUpdateStructure");
                    }
                }
            }
        }

        //TOOD: See if this should be moved to separate thread
        public void ConnectToGateway()
        {
            if (_voiceStateUpdateReceived && _voiceServerUpdateReceived)
            {
                _waitingForGatewayDetails = false;
                if (_endpoint != null && _endpoint != null && _token != null)
                {
                    _voiceWebSocket?.Close();
                    _voiceWebSocket = new WebSocket(_endpoint);
                    _voiceWebSocket.MessageReceived += OnEventReceived;
                    _voiceWebSocket.Opened += OnGatewayOpened;
                    _voiceWebSocket.Closed += OnGatewayClosed;
                    _voiceWebSocket.Error += OnError;
                    _voiceWebSocket.Open();
                    _logger.LogInfo($"Voice gateway opened on endpoint: {_endpoint}");
                }
                else
                {
                    _logger.LogWarning("DiscordVoiceGatewayClient did not have enough info to connect to gateway");
                }
            }
        }

        public void OnEventReceived(object? sender, MessageReceivedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Message))
                {
                    DiscordVoiceGatewayEvent? gatewayEvent = JsonConvert.DeserializeObject<DiscordVoiceGatewayEvent>(e.Message);
                    if (gatewayEvent != null)
                    {
                        HandleOpcode(gatewayEvent);

                        _logger.LogDebug("Received Voice Event:\n" + gatewayEvent.ToJson(Formatting.Indented));
                    }
                }
                else
                {
                    _logger.LogWarning("DiscordGatewayClient received empty Gateway Event");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to parse voice gateway event:\n{e.Message}");
            }
        }

        public void HandleOpcode(DiscordVoiceGatewayEvent gatewayEvent)
        {
            // Handle Opcodes that can be sent by server
            switch (gatewayEvent.GatewayOpcode)
            {
                case DiscordVoiceGatewayOpcode.Ready:
                    HandleReadyPayload(gatewayEvent);
                    break;
                case DiscordVoiceGatewayOpcode.SessionDescription:
                    break;
                case DiscordVoiceGatewayOpcode.Speaking:
                    break;
                case DiscordVoiceGatewayOpcode.HeartbeatACK:
                    HearbeatAcknowledge(gatewayEvent);
                    break;
                case DiscordVoiceGatewayOpcode.Hello:
                    // Voice Hello payload is identical to regular gateway Hello, for API v3 and up
                    var hello = gatewayEvent.JsonEventData?.ToObject<DiscordHelloPayload>();
                    if (hello != null) { HandleGatewayHello(hello); }
                    break;
                case DiscordVoiceGatewayOpcode.Resumed:
                    break;
                case DiscordVoiceGatewayOpcode.ClientDisconnect:
                    _logger.LogInfo($"Received ClientDisconnect payload:\n{gatewayEvent.ToJson()}");
                    break;
                default:
                    _logger.LogWarning($"Received DiscordVoiceGatewayEvent with unexpected Opcode:\n{gatewayEvent.ToJson()}");
                    break;
            }
        }

        public void SendMessage(string message)
        {
            _logger.LogInfo("Voice Websocket state: " + _voiceWebSocket?.State.ToString());
            _voiceWebSocket?.Send(message);
        }

        public void SendIdentifyPayload()
        {
            try
            {
                if (_guildID != null && _sessionID != null && _token != null)
                {
                    DiscordVoiceGatewayEvent voiceGatewayEvent = new(DiscordVoiceGatewayOpcode.Identify,
                        new DiscordVoiceIdentifyPayload(_guildID, _botID, _sessionID, _token));
                    SendMessage(voiceGatewayEvent.ToJson());
                    _logger.LogDebug($"Voice Gateway Client sent Identify payload:\n{voiceGatewayEvent.ToJson()}");
                    _ioHandler.PrintMessage($"Sent Identify payload:\n{voiceGatewayEvent.ToJson()}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
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
                DiscordVoiceHeartbeatPayload heartbeat = new();
                _lastHeartbeatNonce = heartbeat.Nonce;
                SendMessage(heartbeat.ToJson());
                _heartbeatAcknowledged = false;
                _logger.LogDebug($"Voice Heartbeat sent. Nonce = {_lastHeartbeatNonce}");
            }
            else
            {
                //TODO: Handle missed heartbeats
                _logger.LogWarning("Voice Gateway Client missed a HeartbeatACK");
            }
        }

        private void HearbeatAcknowledge(DiscordVoiceGatewayEvent voiceEvent)
        {
            try
            {
                _logger.LogDebug($"Received Voice HeartbeatACK. Nonce = {voiceEvent.RawEventData?.ToString()}");
                if (voiceEvent.RawEventData != null && (long)voiceEvent.RawEventData == _lastHeartbeatNonce)
                {
                    _heartbeatAcknowledged = true;
                }
                else
                {
                    _logger.LogWarning("HeartbeatACK contained a different nonce value");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        public void HandleGatewayHello(DiscordHelloPayload payload)
        {
            StartHeartbeatTimer(payload.HeartbeatInterval);
        }

        private void HandleReadyPayload(DiscordVoiceGatewayEvent gatewayEvent)
        {
            try
            {
                _logger.LogInfo("Voice Gateway Client recieved Ready payload");
                _ioHandler.PrintMessage("Recieved Ready payload");
                var readyPayload = gatewayEvent.JsonEventData?.ToObject<DiscordVoiceReadyPayload>();
                if (readyPayload != null)
                {
                    _udpIpAddress = readyPayload.IP;
                    _udpPort = readyPayload.Port;
                    if (_udpIpAddress != null && _udpPort > 0)
                    {
                        //TODO: send IP discovery packets: https://discord.com/developers/docs/topics/voice-connections#ip-discovery
                        _voiceUDPClient = new UdpClient(_udpIpAddress, _udpPort);
                        _logger.LogInfo($"UdpClient opened on {_udpIpAddress}:{_udpPort}");
                    }
                }
                else
                {
                    _logger.LogWarning("Could not convert voice event payload to DiscordVoiceReadyPayload");
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        private void OnGatewayOpened(object? sender, EventArgs e)
        {
            _ioHandler.PrintMessage($"GatewayOpened: {sender}");
            SendIdentifyPayload();
        }

        private void OnGatewayClosed(object? sender, EventArgs e)
        {
            try
            {
                _voiceUDPClient?.Close();
                _voiceUDPClient?.Dispose();
                _voiceUDPClient = null;
                WebSocket4Net.ClosedEventArgs? closedEventArgs = e as WebSocket4Net.ClosedEventArgs;
                _logger.LogInfo($"Gateway closed by {sender}. {closedEventArgs?.Code}: {closedEventArgs?.Reason}");
                _ioHandler.PrintMessage($"Gateway closed: {sender}. {closedEventArgs?.Code}: {closedEventArgs?.Reason}");
            }
            catch(Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        public void OnError(object? sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            _logger.LogException(e.Exception, "Voice gateway websocket received an error.");
        }

        private void SendAudioTask()
        {
            //var encoder = OpusMSEncoder.Create()
        }
    }
}