using DiscordBot.Bot.Commands;
using DiscordBot.Startup.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands.Validators
{
    internal class RoleValidator
    {
        private const string ADMIN_ROLE = "admin";
        private const string DEFAULT_ROLE = "default";

        private readonly string _adminID;

        public IEnumerable<string> Roles { get; init; }

        public RoleValidator(IEnumerable<string> roles, IOptions<BotSettingsConfiguration> options)
        {
            Roles = roles;
            _adminID = options.Value.AdminID;
        }

        public bool Validate(CommandParameters command)
        {
            //TODO: Get Roles from external source
            if (Roles.Contains(ADMIN_ROLE))
            {
                return _adminID.Equals(command.SenderID);
            }
            else
            {
                return true;
            }
        }
    }
}
