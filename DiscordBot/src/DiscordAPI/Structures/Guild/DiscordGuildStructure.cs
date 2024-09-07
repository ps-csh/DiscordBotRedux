using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Guild
{
    internal class DiscordGuildStructure : DiscordStructure
    {
        /// <summary>
        /// Constructs a partial guild using just guild ID.
        /// It is assumed this will be created by the unavailable guild json structure
        /// </summary>
        /// <param name="id">The Guild snowflake id</param>
        [JsonConstructor]
        public DiscordGuildStructure(string id)
        {
            GuildID = id;
            GuildName = string.Empty;
            OwnerID = string.Empty;
        }

        [JsonProperty("id")]
        public string GuildID { get; set; }

        [JsonProperty("name")]
        public string GuildName { get; set; }

        [JsonProperty("icon")]
        public string? Icon { get; set; }

        [JsonProperty("icon_hash")]
        public string? IconHash { get; set; }

        [JsonProperty("splash")]
        public string? Splash { get; set; }

        [JsonProperty("discovery_splash")]
        public string? DiscoverySplash { get; set; }

        // Only returned from GET Current User Guilds
        [JsonProperty("owner")]
        public bool? Owner { get; set; }

        [JsonProperty("owner_id")]
        public string OwnerID { get; set; }

        // Only returned from GET Current User Guilds
        [JsonProperty("permissions")]
        public string? Permissions { get; set; }

        //Region - Deprecated

        [JsonProperty("afk_channel_id")]
        public string? AFKChannelID { get; set; }

        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }

        //Roles

        //Emojis

        //Features

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("stickers")]
        public JArray? Stickers { get; set; }

        /// <summary>
        /// Is this guild unavailable
        /// </summary>
        /// <remarks>
        /// Will always be set as true in the READY payload upon connecting
        /// </remarks>
        [JsonProperty("unavailable")]
        public bool? Unavailable { get; set; }
    }
}
