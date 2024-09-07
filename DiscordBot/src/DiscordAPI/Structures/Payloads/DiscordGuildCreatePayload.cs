using DiscordBot.DiscordAPI.Structures.Guild;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot.DiscordAPI.Structures.Payloads
{
    /// <summary>
    /// Fields contained in a GUILD_CREATE payload.
    /// Will contain either a guild object with extra fields, or an unavailable guild object
    /// </summary>
    /// <remarks>
    /// If an available guild structure is received, it can be assumed that all fields below
    /// are not null
    /// <br/><see href="https://discord.com/developers/docs/topics/gateway-events#guild-create"/>
    /// </remarks>
    internal class DiscordGuildCreatePayload : DiscordGuildStructure
    {
        [JsonProperty("joined_at")]
        public string? JoinedAt { get; set; }

        [JsonProperty("large")]
        public bool? Large { get; set; }

        [JsonProperty("member_count")]
        public int? MemberCount { get; set; }

        //voice_states

        // Storing structures as JObjects or JArrays to make it easier to debug
        // This way fields can be individually converted to structures to give more accurate stack traces
        [JsonProperty("members")]
        //public List<DiscordGuildMemberStructure>? Members { get; set; }
        public JArray? Members { get; set; }

        [JsonProperty("channels")]
        public JArray? Channels { get; set; }
        //public List<DiscordChannelStructure>? Channels { get; set; }

        [JsonProperty("threads")]
        public JArray? Threads { get; set; }
        //public List<DiscordChannelStructure>? Threads { get; set; }

        //presences
        [JsonProperty("presences")]
        public JArray? Presences { get; set; }
        //public List<DiscordPresenceUpdateStructure>? Presences { get; set; }

        //stage_instances

        //TODO: guild_scheduled_events 

        [JsonConstructor]
        public DiscordGuildCreatePayload(string id) : base(id) { }
    }
}
