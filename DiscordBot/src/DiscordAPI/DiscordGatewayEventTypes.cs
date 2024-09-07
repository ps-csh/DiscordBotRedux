using DiscordBot.Bot.ServerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace DiscordBot.DiscordAPI
{
    internal static class DiscordGatewayEventTypes
    {
        public const string MESSAGE_CREATE = "MESSAGE_CREATE";
        public const string MESSAGE_UPDATE = "MESSAGE_UPDATE";
        public const string CHANNEL_CREATE = "CHANNEL_CREATE";
        public const string CHANNEL_UPDATE = "CHANNEL_UPDATE";
        public const string CHANNEL_DELETE = "CHANNEL_DELETE";
        public const string THREAD_CREATE = "THREAD_CREATE";
        public const string THREAD_UPDATE = "THREAD_UPDATE";
        public const string THREAD_DELETE = "THREAD_DELETE";
        public const string GUILD_CREATE = "GUILD_CREATE";
        public const string GUILD_UPDATE = "GUILD_UPDATE";
        public const string GUILD_DELETE = "GUILD_DELETE";
        public const string GUILD_EMOJIS_UPDATE = "GUILD_EMOJIS_UPDATE";
        public const string GUILD_STICKERS_UPDATE = "GUILD_STICKERS_UPDATE";
        public const string GUILD_MEMBER_ADD = "GUILD_MEMBER_ADD";
        public const string GUILD_MEMBER_REMOVE = "GUILD_MEMBER_REMOVE";
        public const string GUILD_MEMBER_UPDATE = "GUILD_MEMBER_UPDATE";
        public const string MESSAGE_REACTION_ADD = "MESSAGE_REACTION_ADD";
        public const string MESSAGE_REACTION_REMOVE = "MESSAGE_REACTION_REMOVE";
        public const string PRESENCE_UPDATE = "PRESENCE_UPDATE";
        public const string VOICE_STATE_UPDATE = "VOICE_STATE_UPDATE";
        public const string VOICE_SERVER_UPDATE = "VOICE_SERVER_UPDATE";
    }

    /*Application Command Permissions Update	Application command permission was updated
Auto Moderation Rule Create	Auto Moderation rule was created
Auto Moderation Rule Update	Auto Moderation rule was updated
Auto Moderation Rule Delete	Auto Moderation rule was deleted
Auto Moderation Action Execution	Auto Moderation rule was triggered and an action was executed (e.g. a message was blocked)
Channel Create	New guild channel created
Channel Update	Channel was updated
Channel Delete	Channel was deleted
Channel Pins Update	Message was pinned or unpinned
Thread Create	Thread created, also sent when being added to a private thread
Thread Update	Thread was updated
Thread Delete	Thread was deleted
Thread List Sync	Sent when gaining access to a channel, contains all active threads in that channel
Thread Member Update	Thread member for the current user was updated
Thread Members Update	Some user(s) were added to or removed from a thread
Entitlement Create	Entitlement was created
Entitlement Update	Entitlement was updated or renewed
Entitlement Delete	Entitlement was deleted
Guild Create	Lazy-load for unavailable guild, guild became available, or user joined a new guild
Guild Update	Guild was updated
Guild Delete	Guild became unavailable, or user left/was removed from a guild
Guild Audit Log Entry Create	A guild audit log entry was created
Guild Ban Add	User was banned from a guild
Guild Ban Remove	User was unbanned from a guild
Guild Emojis Update	Guild emojis were updated
Guild Stickers Update	Guild stickers were updated
Guild Integrations Update	Guild integration was updated
Guild Member Add	New user joined a guild
Guild Member Remove	User was removed from a guild
Guild Member Update	Guild member was updated
Guild Members Chunk	Response to Request Guild Members
Guild Role Create	Guild role was created
Guild Role Update	Guild role was updated
Guild Role Delete	Guild role was deleted
Guild Scheduled Event Create	Guild scheduled event was created
Guild Scheduled Event Update	Guild scheduled event was updated
Guild Scheduled Event Delete	Guild scheduled event was deleted
Guild Scheduled Event User Add	User subscribed to a guild scheduled event
Guild Scheduled Event User Remove	User unsubscribed from a guild scheduled event
Integration Create	Guild integration was created
Integration Update	Guild integration was updated
Integration Delete	Guild integration was deleted
Interaction Create	User used an interaction, such as an Application Command
Invite Create	Invite to a channel was created
Invite Delete	Invite to a channel was deleted
Message Create	Message was created
Message Update	Message was edited
Message Delete	Message was deleted
Message Delete Bulk	Multiple messages were deleted at once
Message Reaction Add	User reacted to a message
Message Reaction Remove	User removed a reaction from a message
Message Reaction Remove All	All reactions were explicitly removed from a message
Message Reaction Remove Emoji	All reactions for a given emoji were explicitly removed from a message
Presence Update	User was updated
Stage Instance Create	Stage instance was created
Stage Instance Update	Stage instance was updated
Stage Instance Delete	Stage instance was deleted or closed
Typing Start	User started typing in a channel
User Update	Properties about the user changed
Voice State Update	Someone joined, left, or moved a voice channel
Voice Server Update	Guild's voice server was updated
Webhooks Update	Guild channel webhook was created, update, or deleted
Message Poll Vote Add	User voted on a poll
Message Poll Vote Remove	User removed a vote on a poll*/
}
