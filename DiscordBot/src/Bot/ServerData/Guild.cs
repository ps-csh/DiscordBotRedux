using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.ServerData
{
    internal class Guild
    {
        public string GuildID { get; set; }

        public string GuildName { get; set; }

        public List<User> Members { get; set; }

        public List<Channel> TextChannels { get; set; }

        public List<Channel> VoiceChannels { get; set; }

        public Guild(string guildID)
        {
            GuildID = guildID;
            GuildName = string.Empty;
            Members = [];
            TextChannels = [];
            VoiceChannels = [];
        }
    }
}
