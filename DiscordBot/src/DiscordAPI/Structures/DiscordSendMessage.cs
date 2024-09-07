using Newtonsoft.Json;
using System;

namespace DiscordBot.DiscordAPI.Structures
{

    /// <summary>
    /// Message structure containing data needed to send a message to the Discord API
    /// </summary>
    public class DiscordSendMessageStructure : DiscordStructure
    {
        // Nonce values are random 8 digits numbers for this class
        private const int NONCE_MIN = 100000000;
        private const int NONCE_MAX = 1000000000;

        [JsonProperty("content")]
        public string? Content { get; set; }

        // Nonce can accept an integer or string value
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        // Text-to-speech
        [JsonProperty("tts")]
        public bool? Tts { get; set; }

        //TODO:
        // Up to 10 embeds, max 6000 characters total
        [JsonProperty("embeds")]
        public List<DiscordEmbedObjectStructure> Embeds { get; set; } = [];

        //public AllowedMentions? {get; set;}

        // TODO: structure as AllowedMentions class
        //[JsonProperty("allowed_mentions")]
        //public string? AllowedMentions { get; set; }

        //message_reference

        //components

        // Ids of up to 3 stickers in the server to send
        [JsonProperty("sticket_ids")]
        public List<string>? Stickers { get; set; }

        //Note - property name must be "files[n]", where "n" is the number of files
        //-will probably require custom serializer
        //[JsonProperty("files")]
        //public string? Files { get; set; }

        // Json encoded additional request fields, only for multipart messages
        [JsonProperty("payload_json")]
        public string? Payload { get; set; }

        //Array of partial attachments
        //attachments

        //TODO: Implement Message Flags bitfield: https://discord.com/developers/docs/resources/channel#message-object-message-flags
        [JsonProperty("flags")]
        public int? Flags { get; set; }

        //If true and nonce is present, it will be checked for uniqueness in the past few minutes.
        //If another message was created by the same author with the same nonce, that message will be returned and no new message will be created.
        [JsonProperty("enforce_nonce")]
        public bool? EnforceNonce { get; set; }

        //Note - one of content, embeds, sticker_ids, components, files[n], or poll is required.
        private DiscordSendMessageStructure() { }
        public DiscordSendMessageStructure(string content) { Content = content; }
        public DiscordSendMessageStructure(DiscordEmbedObjectStructure embed) { Embeds = [embed]; }
        //TODO: Multiple Embeds count towards same 6000 character limit, split embeds as needed
        public DiscordSendMessageStructure(params DiscordEmbedObjectStructure[] embeds) { Embeds = [..embeds]; }

        //poll

        public DiscordSendMessageStructure SetNonce()
        {
            Nonce = new Random().Next(NONCE_MIN, NONCE_MAX).ToString();
            return this;
        }
    }
}

/*content?*	string	Message contents (up to 2000 characters)
nonce?	integer or string	Can be used to verify a message was sent (up to 25 characters). Value will appear in the Message Create event.
tts?	boolean	true if this is a TTS message
embeds?*	array of embed objects	Up to 10 rich embeds (up to 6000 characters)
allowed_mentions?	allowed mention object	Allowed mentions for the message
message_reference?	message reference	Include to make your message a reply
components?*	array of message component objects	Components to include with the message
sticker_ids?*	array of snowflakes	IDs of up to 3 stickers in the server to send in the message
files[n]?*	file contents	Contents of the file being sent. See Uploading Files
payload_json?	string	JSON-encoded body of non-file params, only for multipart/form-data requests. See Uploading Files
attachments?	array of partial attachment objects	Attachment objects with filename and description. See Uploading Files
flags?	integer	Message flags combined as a bitfield (only SUPPRESS_EMBEDS and SUPPRESS_NOTIFICATIONS can be set)
enforce_nonce?	boolean	If true and nonce is present, it will be checked for uniqueness in the past few minutes. If another message was created by the same author with the same nonce, that message will be returned and no new message will be created.
poll?	poll request object	A poll!*/
