using DiscordBot.Bot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.src.CustomAttributes
{
    internal abstract class CommandValidatorAttribute : Attribute
    {
        public abstract Task<ValidationResult> Validate(CommandParameters args);
    }
}
