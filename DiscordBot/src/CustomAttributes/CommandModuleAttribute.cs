using DiscordBot.Bot.Commands.Validators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class CommandModuleAttribute : Attribute
    {
        public List<Type>? ModuleValidator { get; set; }
    }
}
