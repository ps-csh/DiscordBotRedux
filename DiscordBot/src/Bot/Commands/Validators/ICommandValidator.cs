using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands.Validators
{
    internal interface ICommandValidator
    {
        bool Validate(CommandParameters command);
    }
}
