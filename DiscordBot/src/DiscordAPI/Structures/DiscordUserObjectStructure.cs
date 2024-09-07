using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures
{
    /// <summary>
    /// A Discord user, not specific to guilds
    /// </summary>
    /// <remarks>
    /// Username and Discriminator may be null in partial user structures, but should not be
    /// null otherwise
    /// <br/>Documentation: <see href="https://discord.com/developers/docs/resources/user#user-object"/>
    /// </remarks>
    internal class DiscordUserObjectStructure : DiscordStructure
    {
        [JsonConstructor]
        public DiscordUserObjectStructure(string id) 
        {
            ID = id;
        }

        public DiscordUserObjectStructure(string iD, string username, string discriminator)
        {
            ID = iD;
            Username = username;
            Discriminator = discriminator;
        }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("discriminator")]
        public string? Discriminator { get; set; }

        /// <summary>
        /// User's display name if set, or application name for bots
        /// </summary>
        [JsonProperty("global_name")]
        public string? GlobalName { get; set; }

        /// <summary>
        /// The user's avatar hash
        /// </summary>
        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonProperty("bot")]
        public bool? Bot { get; set; }

        /// <summary>
        /// If the user is an Official Discord System user
        /// </summary>
        [JsonProperty("system")]
        public bool? System { get; set; }

        /// <summary>
        /// If the user has two factor authentication
        /// </summary>
        [JsonProperty("mfa_enabled")]
        public bool? MFAEnabled { get; set; }

        /// <summary>
        /// User's banner hash
        /// </summary>
        [JsonProperty("banner")]
        public string? Banner { get; set; }

        [JsonProperty("accent_color")]
        public int? AccentColor { get; set; }

        [JsonProperty("locale")]
        public string? Locale { get; set; }

        [JsonProperty("verified")]
        public bool? Verified { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        /// <summary>
        /// The flags on a user's account
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/user#user-object-user-flags"/>
        /// </remarks>
        [JsonProperty("flags")]
        public int? Flags { get; set; }

        /// <summary>
        /// The user's Discord Nitro subscription type
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>None</item>
        /// <item>Nitro Classic</item>
        /// <item>Nitro</item>
        /// <item>Nitro Basic</item>
        /// </list>
        /// </remarks>
        [JsonProperty("premium_type")]
        public int? PremiumType { get; set; }

        /// <summary>
        /// The public flags on a user's account
        /// </summary>
        /// <remarks>
        /// <see href="https://discord.com/developers/docs/resources/user#user-object-user-flags"/>
        /// </remarks>
        [JsonProperty("public_flags")]
        public int? PublicFlags { get; set; }

        //avatar_decoration_data
    }
}
