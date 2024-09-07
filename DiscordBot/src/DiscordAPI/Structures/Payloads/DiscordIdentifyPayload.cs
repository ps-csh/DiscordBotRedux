using DiscordBot.DiscordAPI.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Payloads
{
    /// <summary>
    /// Payload associated with OpCode 2: Identify
    /// </summary>
    internal class DiscordIdentifyPayload : DiscordStructure
    {
        public DiscordIdentifyPayload(string token, ConnectionProperties properties,
            int intents, int largeThreshold = 50, bool compress = false)
        {
            Token = token;
            Properties = properties;
            Intents = intents;
            LargeThreshold = largeThreshold;
            Compress = compress;
        }

        /// <summary>
        /// Authentication token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("properties")]
        public ConnectionProperties Properties { get; set; }

        [JsonProperty("large_threshold", NullValueHandling = NullValueHandling.Ignore)]
        public int? LargeThreshold { get; set; }

        [JsonProperty("compress", NullValueHandling = NullValueHandling.Ignore)] //false by default
        public bool? Compress { get; set; }

        //TODO: Implement these payloads
        //Shard - array of two integers, unused since this isn't a public bot
        //Presence

        /// <summary>
        /// Gateway Intents you wish to receive 
        /// </summary>
        [JsonProperty("intents")]
        public int Intents { get; set; }

        public struct ConnectionProperties
        {
            [JsonProperty("os", NullValueHandling = NullValueHandling.Ignore)]
            public string? OS { get; set; }

            [JsonProperty("browser", NullValueHandling = NullValueHandling.Ignore)]
            public string? Browser { get; set; }

            [JsonProperty("device", NullValueHandling = NullValueHandling.Ignore)]
            public string? Device { get; set; }
        }
    }
}
