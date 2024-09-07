using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Startup.Configuration
{
    internal class DiscordGatewayConfiguration
    {
        /// <summary>
        /// Gateway Websocket to connect to
        /// </summary>
        public string WebSocket { get; init; } = string.Empty;

        /// <summary>
        /// Flags defining which Gateway intents this applicition want to receive
        /// from the Discord Gateway
        /// </summary>
        public int? GatewayIntents { get; set; }
    }
}
