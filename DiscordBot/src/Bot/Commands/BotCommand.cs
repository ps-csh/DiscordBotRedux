using DiscordBot.Bot.Commands.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands
{
    internal class BotCommand
    {
        public delegate Task<CommandResult> CommandDelegate(CommandParameters args);
        public delegate Task<ValidationResult> ValidatorDelegate(CommandParameters args);

        public List<Func<CommandParameters, Task<ValidationResult>>> Validators { get; init; }
        public CommandDelegate Command {  get; init; }

        public string CommandKeyword {  get; init; }

        //TODO: Delete if unused
        public int ExpectedArguments { get; init; }

        //TODO: Will we have anything other than RoleValidators?
        //public List<ValidatorDelegate> Validators { get; init; } = [];

        public BotCommand(string keyword, CommandDelegate command, int expectedArguments = 0)
        {
            CommandKeyword = keyword;
            Command = command;
            Validators = [];
            ExpectedArguments = expectedArguments;
        }

        public async Task<ValidationResult> Validate(CommandParameters args)
        {
            if (Validators.Any())
            {
                var result = await Task.WhenAll(Validators.Select(v => v.Invoke(args)));
                return result.FirstOrDefault(r => !r.Success) ?? ValidationResult.Passed();
            }

            return ValidationResult.Passed();
        }

        public async Task<CommandResult> Execute(CommandParameters args)
        {
            return await Command.Invoke(args);
        }
    }
}
