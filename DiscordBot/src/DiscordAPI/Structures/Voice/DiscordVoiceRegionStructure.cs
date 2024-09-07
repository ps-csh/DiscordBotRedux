using Newtonsoft.Json;

namespace DiscordBot.DiscordAPI.Structures.Voice
{
    /// <summary>
    /// Region to connect to in a voice channel
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/voice#voice-region-object"/>
    /// </remarks>
    internal class DiscordVoiceRegionStructure : DiscordStructure
    {
        [JsonRequired, JsonProperty("id")]
        public required string ID { get; init; }

        [JsonRequired, JsonProperty("name")]
        public required string Name { get; init; }

        [JsonProperty("optimal")]
        public bool Optimal { get; init; }

        [JsonProperty("deprecated")]
        public bool Deprecated { get; init; }

        [JsonProperty("custom")]
        public bool Custom { get; init; }

        [JsonConstructor]
        public DiscordVoiceRegionStructure(string id, string name)
        {
            ID = id;
            Name = name;
        }



        /*  id	        string	    unique ID for the region
            name	    string	    name of the region
            optimal	    boolean	    true for a single server that is closest to the current user's client
            deprecated	boolean	    whether this is a deprecated voice region (avoid switching to these)
            custom	    boolean	    whether this is a custom voice region (used for events/etc)*/
    }
}
