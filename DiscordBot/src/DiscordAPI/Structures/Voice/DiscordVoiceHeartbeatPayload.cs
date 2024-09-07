using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    /// <summary>
    /// Payload structure used by both voice gateway Heartbeat and HeartbeatACK payloads
    /// </summary>
    internal class DiscordVoiceHeartbeatPayload : DiscordStructure
    {
        // TODO: confirm acceptable nonce values for voice gateway, Discord example has 13 digits
        [JsonIgnore]
        private const int NONCE_MIN = 0;
        [JsonIgnore]
        private const int NONCE_MAX = int.MaxValue;

        [JsonProperty("op")]
        public int Opcode { get; set; }

        /// <summary>
        /// Integer nonce value sent by client, and should be matched by
        /// server acknowledgement
        /// </summary>
        [JsonProperty("d")]
        public int? Nonce { get; set; }

        [JsonConstructor]
        public DiscordVoiceHeartbeatPayload(int opcode, int? nonce)
        {
            Opcode = opcode;
            Nonce = nonce;
        }

        public DiscordVoiceHeartbeatPayload()
        {
            Opcode = (int)DiscordVoiceGatewayOpcode.Heartbeat;
            SetNonce();
        }

        public void SetNonce()
        {
            Nonce = new Random().Next(NONCE_MIN, NONCE_MAX);
        }
    }
}
