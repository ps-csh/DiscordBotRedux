using Newtonsoft.Json;
using DiscordBot.DiscordAPI.Structures.Guild;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    /// <summary>
    /// Used to represent a user's voice connection status.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/voice"/>
    /// </remarks>
    internal class DiscordVoiceStateStructure : DiscordStructure
    {
        [JsonProperty("guild_id")]
        public string? GuildID { get; init; }

        [JsonProperty("channel_id")]
        public string? ChannelID { get; init; }

        [JsonRequired, JsonProperty("user_id")]
        public string UserID { get; init; }

        [JsonProperty("member")]
        public DiscordGuildMemberStructure? GuildMember { get; init; }

        /// <summary>
        /// Session ID should not be null when recieved by the API,
        /// but will be null when sending initial voice update to join a channel.
        /// <br/>See <see cref="DiscordGatewayOpcode.VoiceStateUpdate"/>
        /// </summary>
        [JsonProperty("session_id")]
        public string? SessionID { get; init; }

        [JsonProperty("deaf")]
        public bool Deaf { get; init; }

        [JsonProperty("mute")]
        public bool Mute { get; init; }

        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; init; }

        [JsonProperty("self_mute")]
        public bool SelfMute { get; init; }

        [JsonProperty("self_stream")]
        public bool? SelfStream { get; init; }

        [JsonProperty("self_video")]
        public bool SelfVideo { get; init; }

        [JsonProperty("suppress")]
        public bool Suppress { get; init; }

        [JsonProperty("request_to_speak_timestamp")]
        public string? RequestToSpeakTimestamp { get; init; }

        /// <summary>
        /// This constructor is used by the JSON Serializer
        /// </summary>
        /// <param name="userId">The ID of the user joining a voice channel</param>
        [JsonConstructor]
        private DiscordVoiceStateStructure(string userId)
        {
            UserID = userId;
        }

        /// <summary>
        /// Constructs a new Voice State object. Channel ID is required for joining
        /// a voice channel.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channelId"></param>
        /// <param name="guildId"></param>
        public DiscordVoiceStateStructure(string userId, string channelId, string? guildId = null)
        {
            UserID = userId;
            ChannelID = channelId;
            GuildID = guildId;
        }

        /*  guild_id?	                snowflake	        the guild id this voice state is for
            channel_id	                ?snowflake	        the channel id this user is connected to
            user_id	                    snowflake	        the user id this voice state is for
            member?	                    guild member object	the guild member this voice state is for
            session_id	                string	            the session id for this voice state
            deaf	                    boolean	            whether this user is deafened by the server
            mute	                    boolean	            whether this user is muted by the server
            self_deaf	                boolean	            whether this user is locally deafened
            self_mute	                boolean	            whether this user is locally muted
            self_stream?                boolean	            whether this user is streaming using "Go Live"
            self_video	                boolean	            whether this user's camera is enabled
            suppress	                boolean	            whether this user's permission to speak is denied
            request_to_speak_timestamp  ?ISO8601 timestamp	the time at which the user requested to speak*/
    }
}
