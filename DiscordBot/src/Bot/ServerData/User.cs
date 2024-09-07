using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.ServerData
{
    internal class User
    {
        public required string ID { get; init; }

        public string? Username { get; set; }

        public string? Nickname { get; set; }

        public string? AvatarHash { get; set; }

        public List<string> BotCommandRoles { get; init; } = [];
    }
}
