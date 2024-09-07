using DiscordBot.DiscordAPI.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.src.DiscordAPI.Structures
{
    internal class DiscordGatewayPresenceUpdate : DiscordStructure
    {
        /// <summary>
        /// Unix time (in milliseconds) of when the client went idle, or null if the client is not idle
        /// </summary>
        [JsonProperty("since")]
        public int? Since { get; set; }

        /// <summary>
        /// User's new status
        /// </summary>
        //activities - requires activity structure

        /// <summary>
        /// User's new status
        /// 
        ///     <list type="bullet">
        ///         <listheader>
        ///         <term>Status Types</term>
        ///         <description>description</description>
        ///         </listheader>
        ///         <item><term>online</term><description>Online</description></item>
        ///         <item><term>dnd</term><description>Do Not Disturb</description></item>
        ///         <item><term>idle</term><description>AFK</description></item>
        ///         <item><term>invisible</term><description>Invisible and shown as offline</description></item>
        ///         <item><term>offline</term><description>Offline</description></item>
        ///     </list>
        /// </summary>
        [JsonProperty("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Whether or not the client is afk
        /// </summary>
        [JsonProperty("afk")]
        public bool? Afk { get; set; }
    }
}
