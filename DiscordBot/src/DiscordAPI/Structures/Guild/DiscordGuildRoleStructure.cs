using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Guild
{
    /// <summary>
    /// <see href="https://discord.com/developers/docs/topics/permissions#role-object"/>
    /// </summary>
    internal class DiscordGuildRoleStructure
    {
        [JsonProperty("id")]
        public required string ID { get; init; }

        [JsonProperty("name")]
        public required string Name { get; init; }

        // Hexadecimal integer representation of Color
        // If 0, it does not affect the final displayed color
        [JsonProperty("color")]
        public int Color { get; init; }

        // Display this role separately from other roles
        [JsonProperty("hoist")]
        public bool Hoist { get; init; }

        [JsonProperty("icon")]
        public string? IconHash { get; init; }

        [JsonProperty("unicode_emoji")]
        public string? UnicodeEmoji { get; init; }

        [JsonProperty("position")]
        public int Position { get; init; }

        // Bit set of role permissions TODO: Find expected value, Discord lists typet as string
        [JsonProperty("permissions")]
        public string Permissions { get; init; } = string.Empty;

        [JsonProperty("mentionable")]
        public bool Mentionable { get; init; }

        // Whether this Role is managed by an integration
        [JsonProperty("managed")]
        public bool Managed { get; init; }

        //RoleTags - these tags will be present and set to null if they are TRUE
        // this will probably require a custom deserializer to parse properly

        //The only flag is IN_PROMPT (1 << 0) - which is only used for onboarding process
        [JsonProperty("flags")]
        public int Flags { get; init; }
    }
}
