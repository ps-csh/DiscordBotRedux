using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Startup.Configuration
{
    internal class AssemblyConfiguration
    {
        /// <summary>
        /// A list of assemblies to load
        /// </summary>
        public List<Assembly> Assemblies { get; init; } = [];

        internal record Assembly(string Name, string Type);
    }
}
