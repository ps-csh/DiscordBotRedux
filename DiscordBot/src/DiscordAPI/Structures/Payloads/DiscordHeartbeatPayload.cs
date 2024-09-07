using DiscordBot.DiscordAPI.Structures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures.Payloads
{
    internal class DiscordHeartbeatPayload : DiscordStructure
    {
        /// <summary>
        /// Payload type, commonly 0 (DISPATCH) for messages
        /// </summary>
        /// <remarks>
        /// For a list of opcodes:
        /// <see href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-opcodes">
        /// Discord API - Gateway Opcodes
        /// </see>
        /// </remarks>
        [JsonProperty("op")]
        public int Opcode { get; set; }

        /// <summary>
        /// Last Sequence received from the Gateway
        /// Can be null if no sequences have been received yet
        /// </summary>
        [JsonProperty("d")]
        public int? LastSequence { get; set; }


        public DiscordHeartbeatPayload(int? lastSequence)
        {
            Opcode = (int)DiscordGatewayOpcode.Heartbeat;
            LastSequence = lastSequence;
        }
    }
}
