using DiscordBot.Bot.Commands;
using DiscordBot.Startup.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands.Modules
{
    internal abstract class CommandModule
    {
        protected CommandHandler _handler;

        public CommandModule(CommandHandler handler)
        {
            _handler = handler;
            _handler.RegisterCommands(this);
        }
    }
}
