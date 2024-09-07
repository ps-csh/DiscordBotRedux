using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.DiscordAPI.Structures
{
    internal class DiscordPresenceUpdateStructure
    {
        /// <summary>
        /// User whose presence is being updated
        /// </summary>
        /// <remarks>
        /// This is a partial user object, but no fields are validated.
        /// Aside from ID, any combinations of user fields can be expected.
        /// </remarks>
        [JsonProperty("user")]
        public DiscordUserObjectStructure User { get; set; }

        /// <summary>
        /// Snowflake ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public string GuildID { get; set; }

        /// <summary>
        /// Either "idle", "dnd", "online", or "offline"
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("activities")]
        public List<DiscordPresenceActivity> Activities { get; set; }

        [JsonConstructor]
        public DiscordPresenceUpdateStructure(DiscordUserObjectStructure user, string guildID, string status, List<DiscordPresenceActivity> activities)
        {
            User = user;
            GuildID = guildID;
            Status = status;
            Activities = activities;
        }

        //client_status



        /*user	user object	User whose presence is being updated
guild_id	snowflake	ID of the guild
status	string	Either "idle", "dnd", "online", or "offline"
activities	array of activity objects	User's current activities
client_status	client_status object	User's platform-dependent status*/
    }
}
