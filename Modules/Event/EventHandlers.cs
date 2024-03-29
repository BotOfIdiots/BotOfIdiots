﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Modules.Chat;
using DiscordBot.Modules.Chat.Class;
using DiscordBot.Modules.Chat.EventHandlers;
using DiscordBot.Modules.DynamicChannels;
using DiscordBot.Modules.Logging;
using DiscordBot.Modules.Logging.Embeds;
using DiscordBot.Modules.Logging.Embeds.Channel;
using DiscordBot.Modules.Logging.Embeds.Member;
using DiscordBot.Modules.Logging.Embeds.Messages;
using DiscordBot.Modules.Logging.Embeds.Punishments;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Modules.Event
{
    public static class EventHandlers
    {
        private static IServiceProvider _serviceProvider = DiscordBot.Services;
        
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
            Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            SocketGuildChannel channel = cachedChannel.Value as SocketGuildChannel;

            if (DbOperations.CheckLogExemption(channel)) return Task.CompletedTask;

            try
            {
                SocketTextChannel logChannel = LogChannels.Messages((channel).Guild);
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
            Cacheable<IMessageChannel, ulong> cachedChannel)
        {
            SocketGuildChannel channel = cachedChannel.Value as SocketGuildChannel;

            if (DbOperations.CheckLogExemption(channel)) return Task.CompletedTask;

            try
            {
                SocketTextChannel logChannel = LogChannels.Messages((channel).Guild);
                if (logChannel == null) return Task.CompletedTask;

                if (cachedData.Count > 0)
                {
                    Embed messageBulkDeleteEmbed =
                        new BulkMessagesDeleted(cachedData, channel as ISocketMessageChannel).Build();

                    logChannel.SendMessageAsync(embed: messageBulkDeleteEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MessageBulkDeleteHandler state");
            }
            catch (Exception e)
            {
                LogException(e, channel.Guild);
                return Task.CompletedTask;
            }
        }

        public static Task MessageUpdateHandler(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message,
            ISocketMessageChannel channel)
        {
            if (cachedMessage.Value.Interaction != null) return Task.CompletedTask;
            if (DbOperations.CheckLogExemption(channel as SocketGuildChannel)) return Task.CompletedTask;

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
                        IRole role = DbOperations.GetJoinRole();
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

        public static Task MemberLeaveGuildHandler(SocketGuild socketGuild, SocketUser socketUser)
        {
            SocketGuildUser leavingUser = socketUser as SocketGuildUser;

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

                throw new Exception("Unhandled MemberLeaveHandler state");
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
                    PrivateChannel.CreatePrivateChannelHandler(stateAfter, user, _serviceProvider).GetAwaiter();
                    PrivateChannel.DestroyPrivateChannelHandler(stateBefore, _serviceProvider).GetAwaiter();
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

        public static Task MemberUpdatedHandler(Cacheable<SocketGuildUser, ulong> before, SocketGuildUser after)
        {
            try
            {
                if (before.Value.Roles.Count != after.Roles.Count)
                {
                    SocketTextChannel logChannel = LogChannels.Roles(before.Value.Guild);
                    if (logChannel == null) return Task.CompletedTask;

                    logChannel.SendMessageAsync(
                        embed: new MemberRolesUpdateEmbedBuilder(after, before.Value.Roles.ToList()).Build()
                    );
                }

                if (before.Value.Nickname != after.Nickname)
                {
                    SocketTextChannel logChannel = LogChannels.Nickname(before.Value.Guild);
                    if (logChannel == null) return Task.CompletedTask;

                    logChannel.SendMessageAsync(
                        embed: new NicknameUpdateEmbedBuilder(after, before.Value.Nickname).Build()
                    );
                }
            }
            catch (Exception e)
            {
                LogException(e, before.Value.Guild);
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Client Event Handlers

        public static Task ClientJoinGuildHandler(SocketGuild guild)
        {
            JoinedGuild.AddGuild(guild, DiscordBot.Services);
            JoinedGuild.DownloadMembers(guild.Users, guild.Id, DiscordBot.Services);
            JoinedGuild.SetGuildOwner(guild.OwnerId, guild.Id, DiscordBot.Services);
            JoinedGuild.GenerateDefaultViolation(guild, DiscordBot.Services);

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

        public static Task ReactionAddedHandler(Cacheable<IUserMessage, ulong> message,
            Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            ISocketMessageChannel channel = cachedChannel.Value as ISocketMessageChannel;

            ReactionMessageEvents.ReactionAdded(message, channel, reaction);
            // Levels.AddReactionXp(message, channel, reaction);

            return Task.CompletedTask;
        }

        public static Task ReactionRemovedHandler(Cacheable<IUserMessage, ulong> message,
            Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            ISocketMessageChannel channel = cachedChannel.Value as ISocketMessageChannel;

            ReactionMessageEvents.ReactionRemoved(message, channel, reaction);
            // Levels.RemoveReactionXp(message, channel, reaction);

            return Task.CompletedTask;
        }
    }
}