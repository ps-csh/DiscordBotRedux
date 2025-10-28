using DiscordBot.Application;
using DiscordBot.Bot;
using DiscordBot.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.src.Controller
{
    internal class ConsoleBotController
    {
        private const string STARTUP_COMMAND = "start";
        private const string STOP_COMMAND = "stop";
        private const string SHUTDOWN_COMMAND = "shutdown";
        private const string HELP_COMMAND = "help";

        private readonly ConsoleIOHandler _handler;
        private readonly BotManager _botManager;
        private readonly ApplicationLifetimeService _applicationLifetime;
        
        private Task<string?>? _task;

        public ConsoleBotController(ConsoleIOHandler consoleIOHandler, BotManager botManager, ApplicationLifetimeService applicationLifetime) 
        {
            _handler = consoleIOHandler;
            _botManager = botManager;
            _applicationLifetime = applicationLifetime;
            _applicationLifetime.RegisterShuttingDownCallback(Shutdown);
        }

        /// <summary>
        /// Polls Command Line for user input
        /// </summary>
        public void PollCommands()
        {
            _task ??= Task.Run(_handler.PollConsole);

            if (_task.IsCompleted)
            {
                string? result = _task.Result;
                if (result != null)
                {
                    HandleCommand(result);
                }
                else
                {
                    _handler.PrintMessage(result ?? "No Input");
                }
                _task = Task.Run(_handler.PollConsole);
            }
        }

        public void HandleCommand(string command)
        {
            switch (command)
            {
                case STARTUP_COMMAND:
                    _handler.PrintMessage("Starting bot");
                    _botManager.StartBot();
                    break;
                case STOP_COMMAND:
                    _handler.PrintMessage("Stopping bot");
                    _botManager.StopBot();
                    break;
                case SHUTDOWN_COMMAND:
                    _handler.PrintMessage("Shutdown command received");
                    _applicationLifetime.Shutdown();
                    break;
                case HELP_COMMAND:
                    _handler.PrintMessage($"{STARTUP_COMMAND}: Starts the BotManager\n" +
                        $"{STOP_COMMAND}: Stops the BotManager\n" +
                        $"{SHUTDOWN_COMMAND}: Terminates the application\n" +
                        $"{HELP_COMMAND}: Displays commands\n");
                    break;
                default:
                    _handler.PrintMessage($"Unknown command: {command}");
                    break;
            }
        }

        public void Shutdown()
        {
            _handler.CancellationTokenSource.Cancel();
        }
    }
}
