using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class CommandAttribute : Attribute
    {
        private readonly string _keyword;
        public string Keyword { get => _keyword; }

        public CommandAttribute(string keyword)
        {
            _keyword = keyword.ToLower().Trim();
        }
    }
}
