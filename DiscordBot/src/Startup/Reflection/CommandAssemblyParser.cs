using DiscordBot.Bot.Commands.Modules;
using DiscordBot.CustomAttributes;
using DiscordBot.Logging;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Startup.Reflection
{
    internal class CommandAssemblyParser
    {
        private List<Assembly> _assemblies;
        private List<DefaultCommandModule> _commandsModules;
        private readonly ILogger _logger;

        public CommandAssemblyParser(IOptions<AssemblyConfiguration> assemblyConfig, ILogger logger)
        {
            _logger = logger;

            //TODO: Parse all assemblies from config
            var defaultAssembly = assemblyConfig.Value.Assemblies.FirstOrDefault();
            if (defaultAssembly != null)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new System.Reflection.AssemblyName(defaultAssembly.Name));
            }
        }

        public IEnumerable<CommandModule>? GetCommandModules(Assembly assembly)
        {
            //var modules = assembly.GetTypes()
            //    .SelectMany(t => t.Get
            return [];
        }

        public IEnumerable<MethodInfo>? GetMethods(Assembly assembly)
        {
            try
            {
                var methods = assembly.GetTypes()
                    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    .Where(t => t.GetCustomAttributes<CommandAttribute>().Any());

                return methods;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }

            return null;
        }
    }
}
