using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands
{
    /// <summary>
    /// The result of executing a command.
    /// Determines output messages upon failure.
    /// </summary>
    /// <param name="result">The type of result after attempting to execute the command</param>
    /// <param name="message">String describing the reason for the result</param>
    internal class CommandResult
    {
        public enum ResultType
        {
            OK,
            Cancelled,
            Denied,
            Failed
        }

        public required ResultType Result { get; init; }
        public required string Message { get; init; }

        public static CommandResult OK()
        {
            return new CommandResult{ Result = ResultType.OK, Message = ""};
        }

        public static CommandResult Cancelled(string message)
        {
            return new CommandResult { Result = ResultType.Cancelled, Message = message };
        }

        public static CommandResult Denied(string message)
        {
            return new CommandResult { Result = ResultType.Denied, Message = message };
        }

        public static CommandResult Failed(string message)
        {
            return new CommandResult { Result = ResultType.Failed, Message = message };
        }
    }
}
