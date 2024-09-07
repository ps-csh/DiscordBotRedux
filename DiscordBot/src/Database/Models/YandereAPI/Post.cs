using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBot.Database.Models.YandereAPI
{
    [Table("YanderePosts")]
    public class YandereAPIPost : DbObject
    {
        public required string PostId { get; set; }
        public int? PoolId { get; set; }

        public required string FileUrl { get; set; }

        // Ratings are character codes (s - Safe, q - Questionable, e - Explicit?)
        public required string Rating { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = [];
        public virtual YandereAPIPool? Pool { get; set; }
    }
}
