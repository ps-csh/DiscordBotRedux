using DiscordBot.DiscordAPI.Structures.Guild;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordBot.DiscordAPI.Structures
{
    /// <summary>
    /// Message structure containing the event data that is received in MESSAGE_CREATE events.
    /// Event data is usually the field "d" in the Json paylod
    /// </summary>
    /// <remarks>
    /// Documentation: <see href="https://discord.com/developers/docs/resources/channel#message-object"/>
    /// </remarks>
    internal class DiscordMessageObjectStructure : DiscordStructure
    {
        /// <summary>
        /// Message ID
        /// </summary>
        /// <remarks>
        /// Type: snowflake
        /// </remarks>
        public string ID { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelID { get; set; }

        [JsonProperty("guild_id")] //May be null
        public string? GuildID { get; set; }

        /// <summary>
        /// The author of this message.
        /// Not guaranteed to be a valid user, such as messages
        /// created through webhooks.
        /// </summary>
        [JsonProperty("author")]
        public DiscordUserObjectStructure Author { get; set; }

        /// <summary>
        /// This field will exist in MESSAGE_CREATE and MESSAGE_UPDATE events
        /// sent from text-based guilds.
        /// </summary>
        [JsonProperty("member")]
        public DiscordGuildMemberStructure? Member { get; set; }

        /// <summary>
        /// ISO8601 timestamp of when the message was created
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// ISO8601 timestamp of when message was last edited
        /// </summary>
        [JsonProperty("edited_timestamp")]
        public string? EditedTimestamp { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
        // Text-to-speech
        [JsonProperty("tts")]
        public bool Tts { get; set; }

        [JsonProperty("mention_everyone")]
        public bool MentionsEveryone { get; set; }

        [JsonProperty("mentions")]
        public List<DiscordUserObjectStructure>? Mentions { get; set; }

        /// <summary>
        /// Array of mentioned Role IDs
        /// </summary>
        [JsonProperty("mention_roles")]
        public List<string> MentionRoles { get; set; } = [];

        //[JsonProperty("mention_channels")]
        //public List<DiscordMentionChannel>? MentionChannels { get; set; }

        //[JsonProperty("attachments")]
        //public List<Attachment> Attachments { get; set; }

        //[JsonProperty("embeds")]
        //public List<Embed> Embeds { get; set; }

        //Reactions

        /// <summary>
        /// For validating when a message was sent. Can be an integer or string
        /// </summary>
        [JsonProperty("nonce")]
        public string? Nonce { get; set; }

        [JsonProperty("pinned")]
        public bool Pinned { get; set; }

        [JsonProperty("webhook_id")]
        public string? WebhookID { get; set; }

        /// <summary>
        /// The type of message
        /// </summary>
        /// <remarks>
        /// For list of types:
        /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-types">
        /// Discord Documentation - Message Types
        /// </see>
        /// </remarks> 
        [JsonProperty("type")]
        public int Type { get; set; }

        //TODO:
        //Activity

        //Application

        //MessageReference

        //Flags

        //Stickers

        //ReferencedMessage

        //TODO: Set mentions, mention_roles, and any other non-nullable field
        public DiscordMessageObjectStructure(string id, string channel_id, DiscordUserObjectStructure author, 
            string content, string timestamp, bool tts, bool mention_everyone, int type)
        {
            ID = id;
            ChannelID = channel_id;
            Author = author;
            Content = content;
            Timestamp = timestamp;
            Tts = tts;
            MentionsEveryone = mention_everyone;
            Type = type;
        }

        /**
         *  id	                        snowflake	id of the message
            channel_id	                snowflake	id of the channel the message was sent in
            author*	                    user object	the author of this message (not guaranteed to be a valid user, see below)
            content**	                string	contents of the message
            timestamp	                ISO8601 timestamp	when this message was sent
            edited_timestamp	        ?ISO8601 timestamp	when this message was edited (or null if never)
            tts	                        boolean	whether this was a TTS message
            mention_everyone	        boolean	whether this message mentions everyone
            mentions	                array of user objects	users specifically mentioned in the message
            mention_roles	            array of role object ids	roles specifically mentioned in this message
            mention_channels?***	    array of channel mention objects	channels specifically mentioned in this message
            attachments**	            array of attachment objects	any attached files
            embeds**	                array of embed objects	any embedded content
            reactions?	                array of reaction objects	reactions to the message
            nonce?	                    integer or string	used for validating a message was sent
            pinned	                    boolean	whether this message is pinned
            webhook_id?	                snowflake	if the message is generated by a webhook, this is the webhook's id
            type	                    integer	type of message
            activity?	                message activity object	sent with Rich Presence-related chat embeds
            application?	            partial application object	sent with Rich Presence-related chat embeds
            application_id?	            snowflake	if the message is an Interaction or application-owned webhook, this is the id of the application
            message_reference?	        message reference object	data showing the source of a crosspost, channel follow add, pin, or reply message
            flags?	                    integer	message flags combined as a bitfield
            referenced_message?****	    ?message object	the message associated with the message_reference
            interaction_metadata?	    message interaction metadata object	In preview. Sent if the message is sent as a result of an interaction
            interaction?	            message interaction object	Deprecated in favor of interaction_metadata; sent if the message is a response to an interaction
            thread?	                    channel object	the thread that was started from this message, includes thread member object
            components?**	            array of message components	sent if the message contains components like buttons, action rows, or other interactive components
            sticker_items?	            array of message sticker item objects	sent if the message contains stickers
            stickers?	                array of sticker objects	Deprecated the stickers sent with the message
            position?	                integer	A generally increasing integer (there may be gaps or duplicates) that represents the approximate position of the message in a thread, it can be used to estimate the relative position of the message in a thread in company with total_message_sent on parent thread
            role_subscription_data?	    role subscription data object	data of the role subscription purchase or renewal that prompted this ROLE_SUBSCRIPTION_PURCHASE message
            resolved?	                resolved data	data for users, members, channels, and roles in the message's auto-populated select menus
            poll?**	                    poll object	A poll!
            call?	                    message call object	the call associated with the message
         */

        //** An app will receive empty values in the content, embeds, attachments,
        //and components fields while poll will be omitted if they have not configured
        //(or been approved for) the MESSAGE_CONTENT privileged intent (1 << 15).
    }

}
