using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands
{
    internal class MentionResponse
    {
        public delegate Task<CommandResult> CommandDelegate(CommandParameters args);

        public string Pattern { get; init; }

        public CommandDelegate Command { get; init; }

        public MentionResponse(string pattern, CommandDelegate commandDelegate)
        {
            Pattern = pattern;
            Command = commandDelegate;
        }

        public bool MatchPattern(string message, out Match match)
        {
            match = Regex.Match(Pattern, message);
            return match.Success;
        }

        public async Task<CommandResult> Execute(CommandParameters args)
        {
            return await Command.Invoke(args);
        }
    }
}
