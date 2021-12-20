using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Objects.Embeds;
using DiscordBot.Objects.Embeds.Channel;
using DiscordBot.Objects.Embeds.Member;
using DiscordBot.Objects.Embeds.Messages;
using DiscordBot.Objects.Embeds.Punishments;

namespace DiscordBot.Modules
{
    public static class EventHandlers
    {
        #region Exception Event Handlers

        public static Task LogException(Exception exception, SocketGuild guild)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Exceptions(guild);
                if (logChannel == null) return Task.CompletedTask;

                Embed exceptionEmbed = new ExceptionLog(exception).Build();

                logChannel.SendMessageAsync(embed: exceptionEmbed);
                return Task.CompletedTask;
            }

            #region Exceptions

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("This exception could not bet logged to the exception channel");
                Console.WriteLine(exception.ToString());
                return Task.CompletedTask;
            }

            #endregion
        }

        #endregion

        #region Message Event Handlers

        public static Task MessageDeleteHandler(Cacheable<IMessage, ulong> cachedMessage,
            ISocketMessageChannel channel)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Messages((channel as SocketTextChannel).Guild);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }

                Embed messageDeleteEmbed = new MessageDeleted(cachedMessage).Build();

                logChannel.SendMessageAsync(embed: messageDeleteEmbed);
                return Task.CompletedTask;
            }

            #region Exceptions

            catch (Exception e)
            {
                LogException(e, (channel as SocketTextChannel).Guild);
                return Task.CompletedTask;
            }

            #endregion
        }

        public static Task MessageBulkDeleteHandler(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedData,
            ISocketMessageChannel channel)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Messages((channel as SocketTextChannel).Guild);
                if (logChannel == null) return Task.CompletedTask;

                if (cachedData.Count > 0)
                {
                    Embed messageBulkDeleteEmbed = new BulkMessagesDeleted(cachedData, channel).Build();

                    logChannel.SendMessageAsync(embed: messageBulkDeleteEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MessageBulkDeleteHandler state");
            }
            catch (Exception e)
            {
                LogException(e, (channel as SocketTextChannel).Guild);
                return Task.CompletedTask;
            }
        }

        public static Task MessageUpdateHandler(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message,
            ISocketMessageChannel channel)
        {
            try
            {
                if (cachedMessage.Value.Content == message.Content) return Task.CompletedTask;

                SocketTextChannel logChannel = LogChannels.Messages((channel as SocketTextChannel).Guild);
                if (logChannel == null) return Task.CompletedTask;

                Embed messageDeleteEmbed = new MessageUpdated(cachedMessage, message).Build();

                logChannel.SendMessageAsync(embed: messageDeleteEmbed);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                LogException(e, (channel as SocketTextChannel).Guild);
                return Task.CompletedTask;
            }
        }

        #endregion

        #region Member Event Handlers

        public static Task MemberJoinGuildHandler(SocketGuildUser joinedUser)
        {
            try
            {
                if (joinedUser != null)
                {
                    DbOperations.InsertUser(joinedUser.Id, joinedUser.Guild);

                    if (DbOperations.CheckJoinRole(joinedUser.Guild))
                    {
                        IRole role = joinedUser.Guild.GetRole(
                            Convert.ToUInt64(DiscordBot.Config["JoinRole"])
                        );
                        joinedUser.AddRoleAsync(role);
                    }

                    SocketTextChannel logChannel = LogChannels.JoinLeave(joinedUser.Guild);
                    if (logChannel == null)
                    {
                        return Task.CompletedTask;
                    }

                    Embed memberJoinEmbed = new JoinLeave(joinedUser, false).Build();

                    logChannel.SendMessageAsync(embed: memberJoinEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MemberJoinHandler state");
            }
            catch (Exception e)
            {
                LogException(e, joinedUser.Guild);
                return Task.CompletedTask;
            }
        }

        public static Task MemberLeaveGuildHandler(SocketGuildUser leavingUser)
        {
            try
            {
                if (leavingUser != null)
                {
                    SocketTextChannel logChannel = LogChannels.JoinLeave(leavingUser.Guild);
                    if (logChannel == null)
                    {
                        return Task.CompletedTask;
                    }

                    Embed memberJoinEmbed = new JoinLeave(leavingUser, true).Build();

                    logChannel.SendMessageAsync(embed: memberJoinEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MemberJoinHandler state");
            }
            catch (Exception e)
            {
                LogException(e, leavingUser.Guild);
                return Task.CompletedTask;
            }
        }

        public static Task MemberVoiceStateHandler(SocketUser user, SocketVoiceState stateBefore,
            SocketVoiceState stateAfter)
        {
            SocketGuild guild = null;

            if (stateAfter.VoiceChannel == stateBefore.VoiceChannel) return Task.CompletedTask;

            if (stateAfter.VoiceChannel != null) guild = stateAfter.VoiceChannel.Guild;

            if (stateBefore.VoiceChannel != null) guild = stateBefore.VoiceChannel.Guild;

            #region Private Channels

            try
            {
                if (DbOperations.CheckPrivateChannel(guild))
                {
                    PrivateChannel.CreatePrivateChannelHandler(stateAfter, user).GetAwaiter();
                    PrivateChannel.DestroyPrivateChannelHandler(stateBefore).GetAwaiter();
                }
            }
            catch (Exception e)
            {
                LogException(e, guild);
            }

            #endregion

            #region Voice State Logging

            try
            {
                SocketTextChannel logChannel = LogChannels.Voice(guild);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }

                Embed logEmbed = null;

                if (stateAfter.VoiceChannel == null)
                    logEmbed = new VoiceStateEmbedBuilder(0, user, stateBefore, stateAfter).Build();

                if (stateBefore.VoiceChannel == null)
                    logEmbed = new VoiceStateEmbedBuilder(1, user, stateBefore, stateAfter).Build();

                if (stateAfter.VoiceChannel != null && stateBefore.VoiceChannel != null)
                    logEmbed = new VoiceStateEmbedBuilder(2, user, stateBefore, stateAfter).Build();


                if (logEmbed != null)
                {
                    logChannel.SendMessageAsync(embed: logEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled Voice State");
            }
            catch (Exception e)
            {
                LogException(e, guild);
                return Task.CompletedTask;
            }

            #endregion
        }

        public static Task MemberUpdatedHandler(SocketGuildUser before, SocketGuildUser after)
        {
            try
            {
                if (before.Roles.Count != after.Roles.Count)
                {
                    SocketTextChannel logChannel = LogChannels.Roles(before.Guild);
                    if (logChannel == null) return Task.CompletedTask;

                    logChannel.SendMessageAsync(
                        embed: new MemberRolesUpdateEmbedBuilder(after, before.Roles.ToList()).Build()
                    );
                }

                if (before.Nickname != after.Nickname)
                {
                    SocketTextChannel logChannel = LogChannels.Nickname(before.Guild);
                    if (logChannel == null) return Task.CompletedTask;
                    
                    logChannel.SendMessageAsync(
                        embed: new NicknameUpdateEmbedBuilder(after, before.Nickname).Build()
                    );
                }
            }
            catch (Exception e)
            {
                LogException(e, before.Guild);
            }

            return Task.CompletedTask;
        }

        public static Task ClientJoinGuildHandler(SocketGuild guild)
        {
            JoinedGuild.AddGuild(guild);
            JoinedGuild.DownloadMembers(guild.Users, guild.Id);
            JoinedGuild.SetGuildOwner(guild.OwnerId, guild.Id);

            return Task.CompletedTask;
        }

        #endregion

        #region Member Ban event Hanlders

        public static Task MemberBannedHandler(SocketUser user, SocketGuild guild)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.JoinLeave(guild);
                if (logChannel == null) return Task.CompletedTask;

                logChannel.SendMessageAsync(embed: new Banned(user).Build());
                return Task.CompletedTask;
            }
            #region Exceptions
            catch (Exception e)
            {
                LogException(e, guild);
                return Task.CompletedTask;
            }
            #endregion
        }

        public static Task MemberUnbannedHandler(SocketUser user, SocketGuild guild)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.JoinLeave(guild);
                if (logChannel == null) return Task.CompletedTask;

                logChannel.SendMessageAsync(embed: new Unbanned(user).Build());
                return Task.CompletedTask;
            }
            #region Exceptions
            catch (Exception e)
            {
                LogException(e, guild);
                return Task.CompletedTask;
            }
            #endregion
        }

        #endregion

        #region Channel Event Handlers

        // public static Task ChannelUpdateHandler(SocketChannel channelBefore, SocketChannel channel)
        // {
        //     if (channelBefore == channel)
        //     {
        //         return Task.CompletedTask;
        //     }
        //
        //     if (channel.GetType() == typeof(SocketDMChannel) || channel.GetType() == typeof(SocketGroupChannel))
        //     {
        //         return Task.CompletedTask;
        //     }
        //
        //     SocketGuildChannel guildChannelBefore = channelBefore as SocketGuildChannel;
        //     SocketGuildChannel guildChannel = channel as SocketGuildChannel;
        //
        //     if (guildChannelBefore.Name != guildChannel.Name)
        //     {
        //     }
        //
        //     return Task.CompletedTask;
        // }

        public static Task ChannelDeleteHandler(SocketChannel channel)
        {
            SocketTextChannel logChannel = LogChannels.ChannelUpdates((channel as SocketGuildChannel).Guild);
            if (logChannel == null)
            {
                return Task.CompletedTask;
            }

            if (channel.GetType() == typeof(SocketDMChannel) || channel.GetType() == typeof(SocketGroupChannel))
            {
                return Task.CompletedTask;
            }

            Embed embed = new ChannelDeletedEmbedBuilder(channel as SocketGuildChannel).Build();

            logChannel.SendMessageAsync(embed: embed);

            return Task.CompletedTask;
        }

        public static Task ChannelCreatedHandler(SocketChannel channel)
        {
            SocketTextChannel logChannel = LogChannels.ChannelUpdates((channel as SocketGuildChannel).Guild);
            if (logChannel == null)
            {
                return Task.CompletedTask;
            }

            if (channel.GetType() == typeof(SocketDMChannel) || channel.GetType() == typeof(SocketGroupChannel))
            {
                return Task.CompletedTask;
            }

            Embed embed = new ChannelCreatedEmbedBuilder(channel as SocketGuildChannel).Build();

            logChannel.SendMessageAsync(embed: embed);
            return Task.CompletedTask;
        }
        
        #endregion

        #region Command Event Handlers

        public static Task LogViolation(Embed violationEmbed, SocketGuild guild)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Punishments(guild);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }

                logChannel.SendMessageAsync(embed: violationEmbed);
            }

            #region Exceptions

            catch (Exception e)
            {
                LogException(e, guild);
            }

            #endregion

            return Task.CompletedTask;
        }
        
        public static Task LogExecutedCommand(SocketCommandContext context, SocketUserMessage message)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Commands(context.Guild);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }


                Embed embed = new ExecutedCommand(context, message).Build();
                logChannel.SendMessageAsync(embed: embed);
            }

            #region Exceptions

            catch (Exception e)
            {
                LogException(e, context.Guild);
            }

            #endregion

            return Task.CompletedTask;
        }

        #endregion
    }
}