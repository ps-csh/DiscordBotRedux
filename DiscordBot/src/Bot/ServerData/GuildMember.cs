using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.ServerData
{
    internal class GuildMember
    {
        public User? User { get; set; }
        public string? Nickname { get; set; }

        public List<Role> Roles { get; set; } = [];
    }
}
