using DiscordBot.Database.Models.YandereAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Database.Models
{
    public class Tag : DbObject
    {
        public string Name { get; set; }

        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<Quote> Quotes { get; set; }
        public virtual ICollection<YandereAPIPost> Posts { get; set; }
    }
}
