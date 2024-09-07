using DiscordBot.Bot.Commands;
using DiscordBot.src.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal class CommandAuthorizerAttribute : CommandValidatorAttribute
    {
        public List<string> RoleNames { get; init; }

        //TODO: Get roles from external enum
        //public CommandAuthorizerAttribute(Roles role)
        //{
        //    RoleName = role.ToString().ToLower().Trim();
        //}

        public CommandAuthorizerAttribute(params string[] rolenames)
        {
            RoleNames = rolenames.Select(s => s.ToLower().Trim()).ToList();
        }

        //TODO: Is it poor form for an attribute to contain methods?
        public override Task<ValidationResult> Validate(CommandParameters args)
        {
            if (RoleNames.Any(r => args.Sender?.BotCommandRoles.Contains(r) ?? false))
            {
                return Task.FromResult(ValidationResult.Passed());
            }
            return Task.FromResult(ValidationResult.Failed("User does not have permission to use this command"));
        }
    }
}
