using DiscordBot.DiscordAPI.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures
{
    internal class DiscordStickerStructure
    {
        public enum StickerType : int
        {
            STANDARD = 1,
            GUILD
        }

        public enum StickerFormat: int
        {
            PNG = 1,
            APNG,
            LOTTIE,
            GIF
        }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("pack_id")]
        public string? PackID { get; set; }

        [JsonProperty("name")]
        public string StickerName { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Comma separated list of keywords for autocompleting tags
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        //asset - Deprecated

        /// <summary>
        /// The type of sticker, either STANDARD or GUILD
        /// </summary>
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("format_type")]
        public int FormatType { get; set; }

        [JsonProperty("available")]
        public bool? Available { get; set; }

        [JsonProperty("guild_id")]
        public string? GuildID { get; set; }

        [JsonProperty("user")]
        public DiscordUserObjectStructure? User { get; set; }

        [JsonProperty("sort_value")]
        public int? SortValue { get; set; }

        [JsonIgnore]
        public StickerType StickerTypeAsEnum => (StickerType)Type;

        [JsonIgnore]
        public StickerFormat StickerFormatAsEnum => (StickerFormat)FormatType;

        [JsonConstructor]
        public DiscordStickerStructure(string iD, string stickerName, string? description, string tags, int type, int formatType)
        {
            ID = iD;
            StickerName = stickerName;
            Description = description;
            Tags = tags;
            Type = type;
            FormatType = formatType;
        }
    }
}
