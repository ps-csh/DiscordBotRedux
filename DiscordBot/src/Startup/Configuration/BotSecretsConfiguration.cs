using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Startup.Configuration
{
    internal class BotSecretsConfiguration
    {
        /// <summary>
        /// The type of token used. Likely "Bot" in this case
        /// </summary>
        public string TokenType { get; set; } = string.Empty;

        /// <summary>
        /// App token associated with this bot, used to connect to the Discord gateway
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
