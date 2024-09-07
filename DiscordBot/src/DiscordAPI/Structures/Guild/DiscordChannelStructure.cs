using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Guild
{
    /// <summary>
    /// A channel in a Discord server.
    /// The same channel structure is used for DMs and Threads as well
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#channel-object"/>
    /// </remarks>
    internal class DiscordChannelStructure : DiscordStructure
    {
        [JsonConstructor]
        public DiscordChannelStructure(string id, int type)
        {
            ChannelID = id;
            Type = type;
        }

        [JsonProperty("id")]
        public string ChannelID { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("guild_id")]
        public string? GuildID { get; set; }

        [JsonProperty("position")]
        public int? Position { get; set; }

        //overwrites - array of overwrite objects

        [JsonProperty("name")]
        public string? ChannelName { get; set; }

        [JsonProperty("topic")]
        public string? Topic { get; set; }

        [JsonProperty("nsfw")]
        public bool? Nsfw { get; set; }

        [JsonProperty("last_message_id")]
        public string? LastMessageID { get; set; }

        //bitrate?
        //user_limit?
        //rate_limit_per_user? - also applies to thread creation
        //recipients? - array of user objects - recipients of the DM

        public List<DiscordUserObjectStructure>? Recipients { get; set; }

        //icon? - string? - icon hash of group DM
        //owner_id? - id of the creator of group DM
        //application_id? - only if DM creator is bot-created
        //managed?
        //parent_id? - parent category id for the channel, or channel id for a thread
        //more unused fields...

        [JsonIgnore]
        public DiscordChannelType ChannelType => (DiscordChannelType)Type;
    }
}
