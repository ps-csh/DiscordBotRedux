using DiscordBot.Application;
using DiscordBot.Bot.Commands;
using DiscordBot.Bot.Commands.Validators;
using DiscordBot.CustomAttributes;
using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.DiscordAPI.Utility;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands.Modules
{
    [CommandModuleAttribute()]
    internal class DefaultCommandModule : CommandModule
    {
        ApplicationLifetimeService _lifetimeService;
        DiscordAPIClient _apiClient;
        BotManager _botManager;

        public DefaultCommandModule(ApplicationLifetimeService lifetimeService, DiscordAPIClient apiClient, 
            CommandHandler handler, BotManager botManager): base(handler)
        {
            _lifetimeService = lifetimeService;
            _apiClient = apiClient;
            _botManager = botManager;
        }

        [Command("test")]
        [CommandAuthorizer("admin")]
        public async Task<CommandResult> Test(CommandParameters cmd)
        {
            await _apiClient.PostMessage("test", cmd.ChannelID);
            return CommandResult.OK();
        }

        [Command("info")]
        //[CommandAuthorizer("admin")]
        public async Task<CommandResult> Info(CommandParameters cmd)
        {
            await _apiClient.PostMessage($"{cmd.SenderID}\n{cmd.CommandString}\n{cmd.Sender?.Username}", cmd.ChannelID);
            return CommandResult.OK();
        }

        [Command("shutdown")]
        [CommandAuthorizer("admin")]
        public Task<CommandResult> Shutdown(CommandParameters cmd)
        {
            _lifetimeService.Shutdown();
            return Task.FromResult(CommandResult.OK());
        }

        [Command("embed")]
        [CommandAuthorizer("admin")]
        public async Task<CommandResult> Embed(CommandParameters cmd)
        {
            await _apiClient.PostEmbedMessage(new DiscordEmbedObjectStructure("TestEmbed")
                .SetAuthor("OncomingTraffic")
                .SetDescription("Test Description")
                .SetFooter("Footer")
                .SetImage(@"https://images-ext-1.discordapp.net/external/ghdjthzvKmq94LvA8MAjbZvWb6O7qEdEuweXIucnAuE/https/pbs.twimg.com/media/GQZ4-IBasAAd1Yq.jpg?format=webp&width=757&height=426")
                .SetColor(Color.Azure),
                cmd.ChannelID);
            return CommandResult.OK();
        }

        [Command("joinvc")]
        [CommandAuthorizer("admin")]
        public Task<CommandResult> JoinVoiceChat(CommandParameters cmd)
        {
            _botManager.JoinVoiceChannel("694608615759151175", cmd.DiscordMessage?.GuildID);
            return Task.FromResult(CommandResult.OK());
        }

        [Command("leavevc")]
        [CommandAuthorizer("admin")]
        public Task<CommandResult> LeaveVoiceChat(CommandParameters cmd)
        {
            // See if this leaves channel
            _botManager.LeaveVoiceChat(null, cmd.DiscordMessage?.GuildID);
            return Task.FromResult(CommandResult.OK());
        }


        [GenericResponse("^(hi|hello) weeabot$")]
        public async Task<CommandResult> Greeting(CommandParameters cmd)
        {
            await _apiClient.PostMessage("Hello " + cmd.SenderID, cmd.ChannelID);
            return CommandResult.OK();
        }
    }
}
