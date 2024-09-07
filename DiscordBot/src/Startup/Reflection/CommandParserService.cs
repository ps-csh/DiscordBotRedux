using DiscordBot.Bot.Commands;
using DiscordBot.CustomAttributes;
using DiscordBot.Bot.Commands.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Bot.Commands.Validators;
using DiscordBot.src.CustomAttributes;

namespace DiscordBot.Startup.Reflection
{
    internal class CommandParserService
    {
        public CommandParserService()
        {
            //TODO: Get Modules on startup
        }

        public List<BotCommand> GetCommandMethods(CommandModule parentClass)
        {
            var type = parentClass.GetType();
            var commands = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(m => new CommandInfo(m, m.GetCustomAttribute<CommandAttribute>(), m.GetCustomAttributes<CommandValidatorAttribute>()))
                .Where(m => m.CommandAttribute != null)
                .ToList();

            List<BotCommand> botCommands = [];

            foreach (var commandInfo in commands)
            {
                var commandDelegate = commandInfo.MethodInfo.CreateDelegate<BotCommand.CommandDelegate>(parentClass);

                BotCommand botCommand = new BotCommand(commandInfo.CommandAttribute!.Keyword, commandDelegate);
                // Looping over validators, since LINQ has trouble converting to delegates without explicit casts everywhere
                foreach (var validator in commandInfo.Validators)
                {
                    botCommand.Validators.Add(validator.Validate);
                }
                botCommands.Add(botCommand);
            }

            return botCommands;
        }

        /// <summary>
        /// CommandInfo structure from pa
        /// </summary>
        /// <param name="MethodInfo"></param>
        /// <param name="CommandAttribute"></param>
        /// <param name="Validators"></param>
        private record CommandInfo(MethodInfo MethodInfo, CommandAttribute? CommandAttribute, IEnumerable<CommandValidatorAttribute> Validators);
    }
}
