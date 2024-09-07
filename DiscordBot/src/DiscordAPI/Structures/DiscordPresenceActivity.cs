using DiscordBot.DiscordAPI.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures
{
    internal class DiscordPresenceActivity : DiscordStructure
    {
        /// <summary>
        /// Valid activity types used in Presence Updates
        /// </summary>
        public enum ActivityTypes : int
        {
            /// <summary>
            /// Playing {name}	"Playing Rocket League"
            /// </summary>
            Game = 0,
            /// <summary>
            /// Streaming {details}	"Streaming Rocket League"
            /// </summary>
            Streaming = 1,
            /// <summary>
            /// Listening to {name}	"Listening to Spotify"
            /// </summary>
            Listening = 2,
            /// <summary>
            /// Watching {name}	"Watching YouTube Together"
            /// </summary>
            Watching = 3,
            /// <summary>
            /// {emoji} {state}	":smiley: I am cool"
            /// </summary>
            Custom = 4,
            /// <summary>
            /// Competing in {name}	"Competing in Arena World Champions"
            /// </summary>
            Competing = 5
        }

        /// <summary>
        /// Activity's name
        /// </summary>
        [JsonProperty("name")]
        public string Name {  get; set; }

        /// <summary>
        /// Activity type
        /// </summary>
        [JsonProperty("type")]
        public int ActivityType { get; set; }

        /// <summary>
        /// Stream URL, only validated when type is 1
        /// </summary>
        [JsonProperty("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Unix timestamp (in milliseconds) of when the activity was added to the user's session
        /// </summary>
        /// <remarks>
        /// Discord listed as integer, but it is more than 32-bits
        /// </remarks>
        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        //TODO: Implement remaining fields


        //Bot users are only able to set name, state, type, and url.
        [JsonConstructor]
        public DiscordPresenceActivity(string name, int type)
        {
            Name = name;
            ActivityType = type;
        }
    }

    /*
     * name	string	Activity's name
type	integer	Activity type
url?	?string	Stream URL, is validated when type is 1
created_at	integer	Unix timestamp (in milliseconds) of when the activity was added to the user's session
timestamps?	timestamps object	Unix timestamps for start and/or end of the game
application_id?	snowflake	Application ID for the game
details?	?string	What the player is currently doing
state?	?string	User's current party status, or text used for a custom status
emoji?	?emoji object	Emoji used for a custom status
party?	party object	Information for the current party of the player
assets?	assets object	Images for the presence and their hover texts
secrets?	secrets object	Secrets for Rich Presence joining and spectating
instance?	boolean	Whether or not the activity is an instanced game session
flags?	integer	Activity flags ORd together, describes what the payload includes
buttons?	array of buttons	Custom buttons shown in the Rich Presence (max 2)
     */
}
