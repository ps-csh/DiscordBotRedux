using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class MentionResponseAttribute : Attribute
    {
        public string Pattern { get; init; }

        public MentionResponseAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
}
