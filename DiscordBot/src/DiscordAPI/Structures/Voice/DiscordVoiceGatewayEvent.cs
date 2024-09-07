using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    internal class DiscordVoiceGatewayEvent : DiscordStructure
    {
        /// <summary>
        /// Payload type
        /// </summary>
        /// <remarks>
        /// For a list of opcodes:
        /// <see href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#voice-voice-opcodes">
        /// Discord API - Gateway Opcodes
        /// </see>
        /// </remarks>
        [JsonRequired, JsonProperty("op")]
        public int Opcode { get; set; }

        //TODO: If 'd' can contain arrays then this will need to change to JToken/JContainer
        /// <summary>
        /// Event data ('d') can contain any JSON object.
        /// It should be converted or parsed based on the event type ('t')
        /// </summary>
        /// <remarks>
        /// Event data must be stored as a base object, since it can be a JObject or a
        /// primitive (int for HeartbeatACK), which JsonDeserializer cannot deserialize directly
        /// </remarks>
        [JsonProperty("d")]
        private object? _eventData;

        [JsonIgnore]
        public JObject? JsonEventData { get => _eventData as JObject; }

        [JsonIgnore]
        public object? RawEventData { get => _eventData; }

        /// <summary>
        /// Sequence number used for heartbeats session resuming.
        /// Only used for opcode 0
        /// </summary>
        //[JsonProperty("s")]
        //public int? Sequence { get; set; }

        public DiscordVoiceGatewayEvent() { }

        public DiscordVoiceGatewayEvent(DiscordVoiceGatewayOpcode opcode, DiscordStructure eventData)
        {
            Opcode = (int)opcode;
            _eventData = eventData.ToJObject();
        }

        /// <summary>
        /// Gets the Opcode as a known DiscordGatewayOpcode enum value
        /// </summary>
        [JsonIgnore]
        public DiscordVoiceGatewayOpcode GatewayOpcode => (DiscordVoiceGatewayOpcode)Opcode;
    }
}
