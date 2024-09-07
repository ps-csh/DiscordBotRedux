using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    /// <summary>
    /// <see href="https://discord.com/developers/docs/topics/voice-connections#establishing-a-voice-websocket-connection-example-voice-identify-payload"/>
    /// </summary>
    internal class DiscordVoiceIdentifyPayload : DiscordStructure
    {
        //TODO: Find proper documentation detailing nullability if these fields
        [JsonProperty("server_id")]
        public string ServerID { get; init; }

        [JsonProperty("user_id")]
        public string UserID { get; init; }

        [ JsonProperty("session_id")]
        public string SessionID { get; init; }

        [JsonProperty("token")]
        public string Token { get; init; }

        public DiscordVoiceIdentifyPayload(string serverID, string userID, string sessionID, string token)
        {
            ServerID = serverID;
            UserID = userID;
            SessionID = sessionID;
            Token = token;
        }
    }
}
