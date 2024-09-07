using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    /// <summary>
    /// <see href="https://discord.com/developers/docs/topics/gateway-events#voice-server-update"/>
    /// </summary>
    internal class DiscordVoiceServerUpdateStructure : DiscordStructure
    {
        [JsonRequired, JsonProperty("token")]
        public string Token { get; init; }

        [JsonRequired, JsonProperty("guild_id")]
        public string GuildID { get; init; }

        [JsonProperty("endpoint")]
        public string? Endpoint {  get; init; }

        public DiscordVoiceServerUpdateStructure(string token, string guildId)
        {
            Token = token;
            GuildID = guildId;
        }
    }
}
