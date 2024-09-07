using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot.DiscordAPI.Structures
{
    /// <summary>
    /// Defines a structure used by the Discord API.
    /// Should be able to convert to and from JSON to communicate with Discord API.
    /// </summary>
    public abstract class DiscordStructure
    {
        /// <summary>
        /// Converts this structure to JSON using DefaultValueHandling.Ignore as serializer settings
        /// </summary>
        /// <returns>String representation of the JSON object</returns>
        public virtual string ToJson()
        {
            //TODO: DefaultValueHandling.Ignore also ignores int values with 0, change to ignore nullable values
            var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include };
            return JsonConvert.SerializeObject(this, settings);
        }

        /// <summary>
        /// Converts this structure to JSON string with specified settings and formatting
        /// </summary>
        /// <param name="settings">JSON Serializer settings to use</param>
        /// <param name="formatting">JSON Formatting to use for text writer</param>
        /// <returns>String representation of the JSON object</returns>
        public virtual string ToJson(JsonSerializerSettings settings, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(this, formatting, settings);
        }

        /// <summary>
        /// Converts this structure to JSON string with default settings and specified formatting
        /// </summary>
        /// <param name="formatting">JSON Formatting to use for text writer</param>
        /// <returns>String representation of the JSON object</returns>
        public virtual string ToJson(Formatting formatting = Formatting.None)
        {
            var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Include };
            return ToJson(settings, formatting);
        }

        /// <summary>
        /// Converts this structure to a JObject
        /// </summary>
        /// <returns>JObject representing this structure</returns>
        public virtual JObject ToJObject()
        {
            return JObject.FromObject(this);
        }
    }
}
