using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    /// <summary>
    /// <see href="https://discord.com/developers/docs/topics/voice-connections#establishing-a-voice-websocket-connection-example-voice-ready-payload"/>
    /// </summary>
    internal class DiscordVoiceReadyPayload : DiscordStructure
    {
        [JsonProperty("ssrc")]
        public int SSRC { get; init; }

        [JsonProperty("ip")]
        public string? IP { get; init; }

        [JsonProperty("port")]
        public int Port { get; init; }

        [JsonProperty("modes")]
        public List<string>? Modes { get; init; }

        //heartbeat_interval is an erroneous field and should be ignored

        public DiscordVoiceReadyPayload() { }
    }
}
