using DiscordBot.Bot.Commands.Modules;
using DiscordBot.Bot.ServerData;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.Logging;
using DiscordBot.Startup.Configuration;
using DiscordBot.Startup.Reflection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands
{
    internal class CommandHandler
    {
        private const string MISSING_COMMAND_RESPONSE = "???";
        private const string UNKNOWN_COMMAND_RESPONSE = "{0} is not a command";
        private const string NOT_ENOUGH_ARGS_REPONSE = "Command: {0} requires {1} arguments";

        private readonly Dictionary<string, BotCommand> _commands;
        private readonly string _botID;
        private readonly List<string> _commandIdentifiers;
        private readonly CommandParserService _commandParserService;
        private readonly DiscordAPIClient _apiClient;
        private readonly DiscordDataRepository _dataRepository;
        private readonly ILogger _logger;

        public ImmutableDictionary<string, BotCommand> Commands => _commands.ToImmutableDictionary();

        public CommandHandler(IOptions<BotSettingsConfiguration> botSettings, DiscordGatewayClient gatewayClient,
            DiscordAPIClient apiClient, CommandParserService commandParserService, DiscordDataRepository repository, ILogger logger)
        {
            _commands = [];
            _apiClient = apiClient;
            gatewayClient.RegisterMessageReceivedCallback(OnMessageReceived);
            _botID = botSettings.Value.BotID;
            _commandIdentifiers = botSettings.Value.CommandIdentifiers;
            _commandParserService = commandParserService;
            _dataRepository = repository;
            _logger = logger;
        }

        public void RegisterCommands(CommandModule commandModule)
        {
            var commands = _commandParserService.GetCommandMethods(commandModule);
            foreach (var command in commands)
            {
                _commands.Add(command.CommandKeyword, command);
                _logger.LogInfo($"Add Command: {command.CommandKeyword}");
            }
        }

        private void OnMessageReceived(DiscordGatewayEvent gatewayEvent)
        {
            if (gatewayEvent.EventType == DiscordGatewayEventTypes.MESSAGE_CREATE)
            {
                _logger.LogInfo($"Received MESSAGE_CREATE in CommandHandler");
                var messagePayload = gatewayEvent.EventData?.ToObject<DiscordMessageObjectStructure>();
                if (messagePayload != null && messagePayload.Author.ID != _botID)
                {
                    if (TryGetCommand(messagePayload.Content, out string command))
                    {
                        HandleCommand(command, messagePayload);
                    }
                }
            }
        }

        private CommandParameters? ParseDiscordMessageObject(DiscordMessageObjectStructure discordMessage)
        {
            // Don't respond to message from self
            if (discordMessage.Author.ID != _botID)
            {
                string commandString = discordMessage.Content.Trim().ToLower();
                if (TryGetCommand(commandString, out string command))
                {
                    //discordMessage.Content.Split()
                    CommandParameters args = new CommandParameters(command, discordMessage.ChannelID, discordMessage.Author.ID)
                    {
                        Sender = _dataRepository.Users.GetValueOrDefault(discordMessage.Author.ID),
                    };
                    return args;
                }
            }

            return null;
        }

        /// <summary>
        /// Parses a message for a command identifier and removes it from the string
        /// </summary>
        /// <param name="message">The string to parse</param>
        /// <param name="command">Out string containing substring with rest of command</param>
        /// <returns>true if message started with command identifier</returns>
        private bool TryGetCommand(string message, out string command)
        {
            command = string.Empty;
            foreach (var id in _commandIdentifiers)
            {
                if (message.StartsWith(id))
                {
                    command = message.Substring(id.Length).Trim();
                    return true;
                }
            }

            return false;
        }

        public bool HasMention(DiscordMessageObjectStructure message)
        {
            return false;
        }

        public async void HandleCommand(string command, DiscordMessageObjectStructure discordMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(command))
                {
                    await _apiClient.PostMessage(MISSING_COMMAND_RESPONSE, discordMessage.ChannelID);
                }

                string[] commandParts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                // If the command somehow had no content, don't continue
                if (commandParts.Length >= 1)
                {
                    if (_commands.TryGetValue(commandParts[0], out BotCommand? botCommand))
                    {
                        var commandArgs = await CreateCommandParameters(commandParts[0], command, commandParts[1..], botCommand, discordMessage);
                        if (commandArgs != null)
                        {
                            //TODO: Handle commands on a threadpool or similar
                            ExecuteCommand(botCommand, commandArgs);
                        }
                    }
                    else
                    {
                        await _apiClient.PostMessage(string.Format(UNKNOWN_COMMAND_RESPONSE, commandParts[0]), discordMessage.ChannelID);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogException(e, $"Error handling command: {command}");
            }
        }

        private Task<CommandParameters?> CreateCommandParameters(string commandKeyword, string commandString, IEnumerable<string> args, BotCommand botCommand, DiscordMessageObjectStructure discordMessage)
        {
            try
            {
                CommandParameters commandArgs = new CommandParameters(commandKeyword, discordMessage)
                {
                    // TODO: Match args with commandline-like options (ie. !quote +a "quote to add")
                    Args = [..args],
                    CommandString = commandString,
                    Sender = _dataRepository.Users.GetValueOrDefault(discordMessage.Author.ID),
                };

                _logger.LogInfo($"Getting user: {discordMessage.Author.ID}, " +
                    $"{_dataRepository.Users.GetValueOrDefault(discordMessage.Author.ID)?.Username ?? "No User"}");

                return Task.FromResult<CommandParameters?>(commandArgs);
            }
            catch (Exception e)
            {
                _logger.LogException(e);
            }

            return Task.FromResult<CommandParameters?>(null);
        }

        private async void ExecuteCommand(BotCommand command, CommandParameters commandArgs)
        {
            var validationResult = await command.Validate(commandArgs);
            if (validationResult.Success)
            {
                var commandResult = await command.Execute(commandArgs);
                if (commandResult.Result != CommandResult.ResultType.OK)
                {
                    await _apiClient.PostMessage(commandResult.Message, commandArgs.ChannelID);
                }
            }
            else
            {
                await _apiClient.PostMessage(validationResult.Message, commandArgs.ChannelID);
            }
        }
    }  
}
