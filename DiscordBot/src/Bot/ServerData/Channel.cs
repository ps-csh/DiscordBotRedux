using DiscordBot.DiscordAPI.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.ServerData
{
    internal class Channel
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public DiscordChannelType ChannelType { get; set; }
        public string? Topic { get; set; }
        public int Position { get; set; }
        public bool Nsfw { get; set; }
        public string? GuildId { get; set; }

        public Channel(string id)
        {
            Id = id; 
        }
    }
}
