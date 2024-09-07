using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands
{
    internal class ValidationResult
    {
        public required bool Success { get; init; }
        public required string Message { get; init; }

        public static ValidationResult Passed()
        {
            return new ValidationResult { Success = true, Message = string.Empty };
        }

        public static ValidationResult Failed(string message)
        {
            return new ValidationResult { Success = false, Message = message };
        }
    }
}
