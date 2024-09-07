using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Startup.Configuration
{
    internal class BotSettingsConfiguration
    {
        // The Owner of the bot, bypasses command permissions
        public string AdminID { get; init; } = string.Empty;
        public string BotID { get; init; } = string.Empty;
        public List<string> CommandIdentifiers { get; init; } = [];
        public string UserAgent { get; init; } = string.Empty;
    }
}
