using DiscordBot.DiscordAPI;
using DiscordBot.DiscordAPI.Structures;
using DiscordBot.DiscordAPI.Structures.Guild;
using DiscordBot.DiscordAPI.Structures.Payloads;
using DiscordBot.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DiscordBot.Bot.ServerData
{
    /// <summary>
    /// Stores and manages local data for Discord servers.
    /// TODO: Find a better name
    /// </summary>
    internal class DiscordDataRepository
    {
        public Dictionary<string, Guild> Guilds { get; private init; }

        public Dictionary<string, User> Users { get; private init; }

        public Dictionary<string, Channel> DirectMessages { get; private set; }

        private readonly ILogger _logger;

        public DiscordDataRepository(DiscordGatewayClient gatewayClient, ILogger logger)
        {
            Guilds = [];
            Users = [];
            DirectMessages = [];

            gatewayClient.RegisterMessageReceivedCallback(OnGuildCreateReceived);
            _logger = logger;
        }

        public Guild AddGuild(string id)
        {
            if (!Guilds.TryGetValue(id, out Guild? guild))
            {
                guild = new Guild(id);
                Guilds.Add(id, guild);
            }

            return guild;
        }

        public User GetOrCreateUser(string id)
        {
            if (!Users.TryGetValue(id, out User? user))
            {
                user = new User{ID = id};
                Users.Add(id, user);
            }

            return user;
        }

        public void AddUser(User user)
        {
            if (!Users.ContainsKey(user.ID))
            {
                Users.Add(user.ID, user);
            }
        }

        public void OnGuildCreateReceived(DiscordGatewayEvent payload)
        {
            try
            {
                if (payload.EventType == DiscordGatewayEventTypes.GUILD_CREATE)
                {
                    DiscordGuildCreatePayload? guildCreatePayload = payload.EventData?.ToObject<DiscordGuildCreatePayload>();
                    if (guildCreatePayload != null)
                    {
                        ParseGuildCreatePayload(guildCreatePayload);
                    }
                    else
                    {
                        _logger.LogWarning("Could not parse GUILD_CREATE payload\n" + payload.EventData?.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                _logger.LogWarning("Could not parse GUILD_CREATE payload\n" + payload.EventData?.ToString());
            }
        }

        public void ParseGuild(DiscordGuildStructure guildStructure)
        {
            string guildID = guildStructure.GuildID;
            if (!Guilds.TryGetValue(guildID, out var guild))
            {
                guild = new Guild(guildID);
                Guilds.Add(guildID, guild);
            }
            if (guildStructure.Unavailable == false)
            {
                guildStructure.GuildName = guildStructure.GuildName;
            }
        }

        public void ParseGuildCreatePayload(DiscordGuildCreatePayload payload)
        {
            try
            {
                var guild = AddGuild(payload.GuildID);
                guild.GuildName = payload.GuildName;
                var channels = payload.Channels?.ToObject<List<DiscordChannelStructure>>();
                if (channels != null)
                {
                    foreach (var ch in channels)
                    {
                        var channel = ParseGuildChannelStructure(ch);
                        if (channel != null)
                        {
                            if (channel.ChannelType == DiscordChannelType.GUILD_VOICE)
                            {
                                guild.VoiceChannels.Add(channel);
                            }
                            else
                            {
                                guild.TextChannels.Add(channel);
                            }
                        }
                    }
                }

                var users = payload.Members?.ToObject<List<DiscordGuildMemberStructure>>();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        var guildMember = ParseGuildMemberStructure(user);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Expection caught while parsing DiscordGuildCreatePayload");
            }
        }

        public Channel ParseGuildChannelStructure(DiscordChannelStructure channelStructure)
        {
            Channel channel = new Channel(channelStructure.ChannelID)
            {
                Name = channelStructure.ChannelName,
                ChannelType = channelStructure.ChannelType,
                Topic = channelStructure.Topic,
                Position = channelStructure.Position ?? 0,
                Nsfw = channelStructure.Nsfw ?? false,
                GuildId = channelStructure.GuildID
            };

            return channel;
        }

        public void ParseRoles()
        {
        }

        public GuildMember ParseGuildMemberStructure(DiscordGuildMemberStructure memberStructure)
        {
            User? user = null;
            if (memberStructure.User != null)
            {
                user = ParseUserStructure(memberStructure.User);
            }

            GuildMember guildMember = new GuildMember()
            {
                Nickname = memberStructure.Nickname,
                User = user,
               
            };
            

            return guildMember;
        }

        public User ParseUserStructure(DiscordUserObjectStructure userStructure)
        {
            User user = GetOrCreateUser(userStructure.ID);
            user.Username = userStructure.Username;
            user.Nickname = userStructure.GlobalName;
            user.AvatarHash = userStructure.Avatar;
            return user;
        }
    }
}
