using DiscordBot.CustomAttributes;
using DiscordBot.Database;
using DiscordBot.Database.Models;
using DiscordBot.DiscordAPI;
using DiscordBot.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands.Modules
{
    internal class DbAccessCommandModule : CommandModule
    {
        /// <summary>
        /// How many times to retry a command if it threw a ConcurrencyException
        /// </summary>
        private const int MAX_CONCURRENCY_RETRIES = 3;

        private readonly BotDbContext _dbContext;
        private readonly DiscordAPIClient _apiClient;
        private readonly ILogger _logger;

        public DbAccessCommandModule(CommandHandler handler, BotDbContext dbContext,
            DiscordAPIClient apiClient, ILogger logger) : base(handler)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _logger = logger;
        }

        //Note: this pattern will bypass validations on other methods, don't use it on unauthorized commands
        [Command("quote")]
        public async Task<CommandResult> QuoteDelegator(CommandParameters cmd)
        {
            switch (cmd.Args.Count)
            {
                case 0:
                    return await GetRandomQuote(cmd);
                case 1:
                    break;
            }

            return CommandResult.OK();
        }

        [Command("randomquote")]
        public async Task<CommandResult> GetRandomQuote(CommandParameters cmd)
        {
            var count = _dbContext.Quotes.Count();
            if (count > 0)
            {
                var index = new Random().Next(count);
                var quote = _dbContext.Quotes.ElementAtOrDefault(index);
                if (quote != null)
                {
                    await _apiClient.PostMessage(quote.Message, cmd.ChannelID);
                    return CommandResult.OK();
                }
                return CommandResult.Failed("Quote was null");
            }
            else
            {
                return CommandResult.Failed("Could not find any quotes");
            }
        }

        [Command("addquote")]
        [CommandAuthorizer("admin")]
        public async Task<CommandResult> AddQuote(CommandParameters cmd)
        {
            if (cmd.Args.Count == 1 || string.IsNullOrWhiteSpace(cmd.Args[0]))
            {
                try
                {
                    await _dbContext.Quotes.AddAsync(new Quote
                    {
                        Message = cmd.Args[0],
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = cmd.SenderID,
                    });

                    int result = await SaveDbChanges();
                    // Assume 1 row is being saved
                    if (result == 1)
                    {
                        await _apiClient.PostMessage("Quote added", cmd.ChannelID);
                        return CommandResult.OK();
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogException(ex, $"Database operation cancelled when adding quote:{cmd.Args[0]}");
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex);
                }
                return CommandResult.Failed("Failed to add quote");
            }

            return CommandResult.Denied("No quote was provided");
        }

        private async Task<int> SaveDbChanges()
        {
            for (int i = 0; i < MAX_CONCURRENCY_RETRIES; ++i)
            {
                try
                {
                    int result = await _dbContext.SaveChangesAsync();
                    return result;
                }
                catch (DBConcurrencyException ex)
                {
                    _logger.LogException(ex, "Concurrency exception when saving DB changes");
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex, "Failed to save DB changes");
                    return -1;
                }
            }

            return -1;
        }
    }
}
