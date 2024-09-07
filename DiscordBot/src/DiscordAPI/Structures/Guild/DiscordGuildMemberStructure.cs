using DiscordBot.Bot.ServerData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Guild
{
    /// <summary>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-member-object"/>
    /// </summary>
    internal class DiscordGuildMemberStructure
    {
        /// <summary>
        /// The Discord User associated with this guild member
        /// </summary>
        /// <remarks>
        /// User is not included in MESSAGE_CREATE and MESSAGE_UPDATE events
        /// </remarks>
        [JsonProperty("user")]
        public DiscordUserObjectStructure? User { get; set; }

        /// <summary>
        /// The member's nickname for this guild, may be null
        /// </summary>
        [JsonProperty("nick")]
        public string? Nickname { get; set; }

        /// <summary>
        /// The member's guild avatar hash
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        /// <summary>
        /// Array of role snowflake ids
        /// </summary>
        [JsonProperty("roles")]
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// ISO8601 timestamp of when the user joined the guild
        /// </summary>
        [JsonProperty("joined_at")]
        public string JoinedAt { get; set; } = "";

        /// <summary>
        /// ISO8601 timestamp of when the user started boosting the guild
        /// </summary>
        [JsonProperty("premium_since")]
        public string? PremiumSince { get; set; }

        /// <summary>
        /// If the user is deafened in voice channels
        /// </summary>
        [JsonProperty("deaf")]
        public bool Deaf { get; set; }

        /// <summary>
        /// If the user is muted in voice channels
        /// </summary>
        [JsonProperty("mute")]
        public bool Mute { get; set; }

        /// <summary>
        /// Guild members flags, related to verification or if the user has left and rejoined.
        /// Likely not relevant for this application.
        /// </summary>
        [JsonProperty("flags")]
        public int Flags { get; set; }

        /// <summary>
        /// If the user has not yet passed the guild's Membership Screening requirements
        /// </summary>
        [JsonProperty("pending")]
        public bool? Pending { get; set; }

        [JsonProperty("permissions")]
        public string? Permissions { get; set; }

        //communication_disabled_until?

        //avatar_decoration_data? - irrelevant to this application
    }
}
