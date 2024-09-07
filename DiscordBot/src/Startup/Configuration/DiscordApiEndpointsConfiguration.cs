using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Startup.Configuration
{
    internal class DiscordApiEndpointsConfiguration
    {
        public string Base { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Guild { get; set; } = string.Empty;
        public string Webhook { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;

        public string ChannelURL => Base + Channel;
        public string MessageURL => Base + Message;
        public string GuildURL => Base + Guild;
        public string WebhookURL => Base + Webhook;
        public string UserURL => Base + User;
    }
}
