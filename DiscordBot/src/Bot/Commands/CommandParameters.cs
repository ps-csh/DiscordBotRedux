using DiscordBot.Bot.ServerData;
using DiscordBot.DiscordAPI.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Bot.Commands
{
    internal class CommandParameters
    {
        //TODO: Is full command string needed anywhere?
        /// <summary>
        /// The full command 
        /// </summary>
        public string CommandString { get; init; }
        /// <summary>
        /// The command keyword, matching the command name from CommandAttribute
        /// </summary>
        public string? CommandKeyword { get; init; }
        //TODO: Parse args from command string using commandline-like values (ie. +d, +s)
        public List<string> Args { get; init; }
        public string ChannelID { get; init; }
        public string SenderID { get; init; }
        public User? Sender { get; init; }
        public DiscordMessageObjectStructure? DiscordMessage { get; init; }

        public CommandParameters(string command, string channelID, string senderID)
        {
            CommandString = command;
            ChannelID = channelID;
            SenderID = senderID;
            Args = [];
        }

        public CommandParameters(string command, DiscordMessageObjectStructure discordMessage)
        {
            CommandString = command;
            DiscordMessage = discordMessage;
            ChannelID = discordMessage.ChannelID;
            SenderID = discordMessage.Author.ID;
            Args = [];
        }
    }
}
