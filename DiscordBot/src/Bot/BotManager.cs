using DiscordBot.Application;
using DiscordBot.Bot.Commands;
using DiscordBot.Bot.ServerData;
using DiscordBot.Database;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.DiscordAPI.Structures.Voice;
using DiscordBot.Logging;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DiscordBot.Bot
{
    internal class BotManager
    {
        private readonly DiscordGatewayClient _gatewayClient;
        private readonly DiscordAPIClient _apiClient;
        private readonly DiscordVoiceGatewayClient _voiceGatewayClient;
        private readonly CommandHandler _commandHandler;
        private readonly DiscordDataRepository _dataRepository;

        private ILogger _logger;
        private readonly User _admin;
        private readonly User _botUser;
        public User Admin { get => _admin; }

        public BotManager(IOptions<BotSettingsConfiguration> botSettings, DiscordGatewayClient discordGatewayClient, 
            DiscordAPIClient apiClient, DiscordVoiceGatewayClient voiceGatewayClient, CommandHandler commandHandler,
            ApplicationLifetimeService lifetimeService, BotDbContext databaseContext, DiscordDataRepository dataRepository, ILogger logger)
        {
            _gatewayClient = discordGatewayClient;
            //_gatewayClient.RegisterMessageReceivedCallback(OnMessageReceived);
            _apiClient = apiClient;
            _voiceGatewayClient = voiceGatewayClient;
            _commandHandler = commandHandler;
            _dataRepository = dataRepository;

            _logger = logger;
            _admin = new() { ID = botSettings.Value.AdminID, BotCommandRoles = ["admin"] };
            _botUser = new() { ID = botSettings.Value.BotID };
            _dataRepository.AddUser(_admin);
            _dataRepository.AddUser(_botUser);

            if (databaseContext == null)
            {
                _logger.LogWarning("DbContext was null");
            }

            lifetimeService.RegisterShuttingDownCallback(OnApplicationShutdown);
        }

        public void OnMessageReceived(DiscordGatewayEvent payload)
        {
            //TODO: Does BotManager need to handle incoming messages?
        }

        public async void StartBot()
        {
            _logger.LogInfo("Connecting websocket from BotManager");
            await _gatewayClient.ConnectWebsocket();
        }

        public async void StopBot()
        {
            _logger.LogInfo("Disconnecting websocket from BotManager");
            await _gatewayClient.DisconnectWebsocket();
        }

        public void OnApplicationShutdown()
        {
            StopBot();
        }

        public void UpdateState()
        {

        }

        public void JoinVoiceChannel(string channelId, string? guildId = null)
        {
            _voiceGatewayClient.PrepareForGatewayConnection(guildId);
            DiscordVoiceStateStructure voiceState = new(_botUser.ID, channelId, guildId);
            _gatewayClient.SendVoiceUpdate(voiceState);
        }

        public void LeaveVoiceChat(string? channelId, string? guildId = null)
        {
            DiscordVoiceStateStructure voiceState = new(_botUser.ID, channelId, guildId);
            _gatewayClient.SendVoiceUpdate(voiceState);
            _voiceGatewayClient.CloseGateway();
        }
    }
}
