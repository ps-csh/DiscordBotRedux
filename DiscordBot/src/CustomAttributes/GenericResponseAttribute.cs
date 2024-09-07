using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class GenericResponseAttribute : Attribute
    {
        public string _messagePattern;

        public GenericResponseAttribute(string messagePattern)
        {
            _messagePattern = messagePattern;
        }
    }
}
