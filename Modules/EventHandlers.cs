using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Models;
using DiscordBot.Models.Embeds;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Modules
{
    public static class EventHandlers
    {
        #region Exception Event Handlers
        public static Task LogException(Exception exception, ulong guildId)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Exceptions(guildId);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                
                Embed exceptionEmbed = new EmbedBuilder
                    {
                        Title = exception.Message
                    }
                    .WithColor(Color.Red)
                    // .WithDescription(exception.StackTrace)
                    // .AddField("Source", exception.Source)
                    .WithFooter(DiscordBot.Version())
                    .WithCurrentTimestamp()
                    .Build();

                logChannel.SendMessageAsync(embed: exceptionEmbed);
                Console.WriteLine(exception.ToString());
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("This exception could not bet logged to the exception channel");
                Console.WriteLine(exception.ToString());
                return Task.CompletedTask;
            }
        }
        #endregion

        #region Message Event Handlers
        public static Task MessageDeleteHandler(Cacheable<IMessage, ulong> cachedMessage,
            ISocketMessageChannel channel)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Messages((channel as SocketTextChannel).Guild.Id);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                
                if (cachedMessage.HasValue)
                {
                    var message = cachedMessage.Value;
                    
                    Embed messageDeleteEmbed = new EmbedBuilder
                        {
                            Title = "Message Deleted"
                        }
                        .WithAuthor(message.Author)
                        .WithColor(Color.Red)
                        .WithDescription(message.Content)
                        .AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")")
                        .AddField("Sent At", message.Timestamp.ToLocalTime()
                            .ToString("HH:mm:ss dd-MM-yyyy"))
                        .WithFooter("MessageID: " + message.Id)
                        .WithCurrentTimestamp()
                        .Build();

                    logChannel.SendMessageAsync(embed: messageDeleteEmbed);
                    Console.WriteLine(logChannel.Name);
                    return Task.CompletedTask;
                }

                if (!cachedMessage.HasValue)
                {
                    Embed messageDeleteEmbed = new EmbedBuilder
                        {
                            Title = "Message Deleted"
                        }
                        .WithColor(Color.Red)
                        .AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")")
                        .WithDescription("Could not retrieve message from cache")
                        .WithFooter("MessageID: " + cachedMessage.Id)
                        .WithCurrentTimestamp()
                        .Build();

                    logChannel.SendMessageAsync(embed: messageDeleteEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Message Unhandled MessageDeleteHandler State");
            }
            catch (Exception e)
            {
                LogException(e, (channel as SocketTextChannel).Guild.Id);
                return Task.CompletedTask;
            }
        }

        public static Task MessageBulkDeleteHandler(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedData,
            ISocketMessageChannel channel)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Messages((channel as SocketTextChannel).Guild.Id);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                
                if (cachedData.Count > 0)
                {
                    Embed messageBulkDeleteEmbed = new EmbedBuilder
                        {
                            Title = "Bulk Messages Delete"
                        }
                        .WithColor(Color.Red)
                        .AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")")
                        .AddField("Amount", cachedData.Count)
                        .WithFooter("Message Count: " + cachedData.Count)
                        .WithCurrentTimestamp()
                        .Build();

                    logChannel.SendMessageAsync(embed: messageBulkDeleteEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MessageBulkDeleteHandler state");
            }
            catch (Exception e)
            {
                LogException(e, (channel as SocketTextChannel).Guild.Id);
                return Task.CompletedTask;
            }
        }

        public static Task MessageUpdateHandler(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message,
            ISocketMessageChannel channel)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Messages((channel as SocketTextChannel).Guild.Id);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                
                if (!cachedMessage.HasValue || message.Content == null)
                {
                    Embed messageDeleteEmbed = new EmbedBuilder
                        {
                            Title = "Message Updated"
                        }
                        .WithColor(Color.Orange)
                        .AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")")
                        .WithDescription("Could not retrieve message from cache")
                        .WithFooter("MessageID: " + cachedMessage.Id)
                        .WithCurrentTimestamp()
                        .Build();

                    logChannel.SendMessageAsync(embed: messageDeleteEmbed);
                    return Task.CompletedTask;
                }

                if (cachedMessage.Value.Content == message.Content)
                {
                    return Task.CompletedTask;
                }

                if (cachedMessage.HasValue)
                {
                    var oldMessage = cachedMessage.Value;

                    Embed messageUpdateEmbed = new EmbedBuilder
                        {
                            Title = "Message Updated"
                        }
                        .WithAuthor(message.Author)
                        .WithColor(Color.Orange)
                        .AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")")
                        .AddField("Old Content", oldMessage.Content)
                        .AddField("New Content", message.Content)
                        .AddField("Sent At", message.Timestamp.ToLocalTime()
                            .ToString("HH:mm:ss dd-MM-yyyy"))
                        .WithFooter("MessageID: " + message.Id)
                        .WithCurrentTimestamp()
                        .Build();

                    logChannel.SendMessageAsync(embed: messageUpdateEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MessageUpdateHandler State");
            }
            catch (Exception e)
            {
                LogException(e, (channel as SocketTextChannel).Guild.Id);
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
                    if (DbOperations.CheckJoinRole())
                    {
                        IRole role = DiscordBot.Client.GetGuild(
                            DiscordBot.GuildId
                        ).GetRole(
                            Convert.ToUInt64(DiscordBot.Config["JoinRole"])
                        );
                        joinedUser.AddRoleAsync(role);
                    }

                    DbOperations.InsertUser(joinedUser.Id, joinedUser.Guild.Id);
                    
                    SocketTextChannel logChannel = LogChannels.JoinLeave(joinedUser.Guild.Id);
                    if (logChannel == null)
                    {
                        return Task.CompletedTask;
                    }

                    Embed memberJoinEmbed = new EmbedBuilder
                        {
                            Title = "Member Joined Server"
                        }
                        .WithAuthor(joinedUser)
                        .AddField("Username", joinedUser.Mention)
                        .AddField("User Created At", joinedUser.CreatedAt.ToLocalTime()
                            .ToString("HH:mm:ss dd-MM-yyyy"))
                        .AddField("User Joined at", joinedUser.JoinedAt?.ToLocalTime()
                            .ToString("HH:mm:ss dd-MM-yyyy"))
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp()
                        .WithFooter("UserID: " + joinedUser.Id)
                        .Build();

                    logChannel.SendMessageAsync(embed: memberJoinEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MemberJoinHandler state");
            }
            catch (Exception e)
            {
                LogException(e, joinedUser.Guild.Id);
                return Task.CompletedTask;
            }
        }

        public static Task MemberLeaveGuildHandler(SocketGuildUser leavingUser)
        {
            try
            {
                if (leavingUser != null)
                {
                    SocketTextChannel logChannel = LogChannels.JoinLeave(leavingUser.Guild.Id);
                    if (logChannel == null)
                    {
                        return Task.CompletedTask;
                    }

                    Embed memberJoinEmbed = new EmbedBuilder
                        {
                            Title = "Member Left Server"
                        }
                        .WithAuthor(leavingUser)
                        .AddField("Username", leavingUser.Mention)
                        .AddField("User Created At", leavingUser.CreatedAt.ToLocalTime()
                            .ToString("HH:mm:ss dd-MM-yyyy"))
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp()
                        .WithFooter("UserID: " + leavingUser.Id)
                        .Build();

                    logChannel.SendMessageAsync(embed: memberJoinEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MemberJoinHandler state");
            }
            catch (Exception e)
            {
                LogException(e, leavingUser.Guild.Id);
                return Task.CompletedTask;
            }
        }

        public static Task MemberVoiceStateHandler(SocketUser user, SocketVoiceState stateBefore,
            SocketVoiceState stateAfter)
        {
            ulong guildId = 0;
            
            if (stateAfter.VoiceChannel == stateBefore.VoiceChannel)
            {
                return Task.CompletedTask;
            }
            
            if (stateAfter.VoiceChannel != null)
            {
                guildId=stateAfter.VoiceChannel.Guild.Id;
            }

            if (stateBefore.VoiceChannel != null)
            {
                guildId=stateBefore.VoiceChannel.Guild.Id;
            }

            try
            {
                if (DiscordBot.Config.GetChildren().Any(item => item.Key == "PrivateChannels"))
                {
                    PrivateChannel.CreatePrivateChannelHandler(stateAfter, user).GetAwaiter();
                    PrivateChannel.DestroyPrivateChannelHandler(stateBefore).GetAwaiter();
                }
            }
            catch (Exception e)
            {
                LogException(e, guildId);
            }

            try
            {
                SocketTextChannel logChannel = LogChannels.Voice(guildId);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                
                Embed logEmbed = null;

                if (stateAfter.VoiceChannel == null)
                {
                    logEmbed = new VoiceStateEmbedBuilder(0, user, stateBefore, stateAfter).Build();
                }

                if (stateBefore.VoiceChannel == null)
                {
                    logEmbed = new VoiceStateEmbedBuilder(1, user, stateBefore, stateAfter).Build();
                }

                if (stateAfter.VoiceChannel != null && stateBefore.VoiceChannel != null)
                {
                    logEmbed = new VoiceStateEmbedBuilder(2, user, stateBefore, stateAfter).Build();
                }

                if (logEmbed != null)
                {
                    logChannel.SendMessageAsync(embed: logEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled Voice State");
            }
            catch (Exception e)
            {
                LogException(e, guildId);
                return Task.CompletedTask;
            }
        }

        public static Task MemberUpdatedHandler(SocketGuildUser before, SocketGuildUser after)
        {
            try
            {
                if (before.Roles.Count != after.Roles.Count)
                {
                    SocketTextChannel logChannel = LogChannels.Roles(before.Guild.Id);
                    if (logChannel == null)
                    {
                        return Task.CompletedTask;
                    }
                    
                    logChannel.SendMessageAsync(
                        embed: new MemberRolesUpdateEmbedBuilder(after, before.Roles.ToList()).Build()
                    );
                }

                if (before.Nickname != after.Nickname)
                {
                    SocketTextChannel logChannel = LogChannels.Nickname(before.Guild.Id);
                    if (logChannel == null)
                    {
                        return Task.CompletedTask;
                    }
                    
                    logChannel.SendMessageAsync(
                        embed: new NicknameUpdateEmbedBuilder(after, before.Nickname).Build()
                    );
                }
            }
            catch (Exception e)
            {
                LogException(e, before.Guild.Id);
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
                SocketTextChannel logChannel = LogChannels.Logs(guild.Id);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }

                logChannel.SendMessageAsync(
                    embed: new EmbedBuilder()
                        .WithTitle("User Banned")
                        .AddField("user", user.Mention)
                        .WithCurrentTimestamp()
                        .WithColor(Color.Red)
                        .WithFooter("UserID: " + user.Id)
                        .Build()
                );

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                LogException(e, guild.Id);
                return Task.CompletedTask;
            }
        }

        public static Task MemberUnbannedHandler(SocketUser user, SocketGuild guild)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Logs(guild.Id);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                
                logChannel.SendMessageAsync(
                    embed: new EmbedBuilder()
                        .WithTitle("User Unbanned")
                        .AddField("user", user.Mention)
                        .WithCurrentTimestamp()
                        .WithColor(Color.Green)
                        .WithFooter("UserID: " + user.Id)
                        .Build()
                );

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                LogException(e, guild.Id);
                return Task.CompletedTask;
            }
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
            SocketTextChannel logChannel = LogChannels.ChannelUpdates((channel as SocketGuildChannel).Guild.Id);
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
            SocketTextChannel logChannel = LogChannels.ChannelUpdates((channel as SocketGuildChannel).Guild.Id);
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

        public static Task LogViolation(Embed violationEmbed, ulong guildId)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Logs(guildId);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                
                logChannel.SendMessageAsync(embed: violationEmbed);
            }
            catch (Exception e)
            {
                LogException(e, guildId);
            }

            return Task.CompletedTask;
        }
        #endregion
        
        #region Reaction Event Handlers
        public static Task ReactionAddedHandler(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            IConfiguration reactionMessages = DiscordBot.Config.GetSection("ReactionMessages");

            if (reactionMessages.GetChildren().Any(item => item.Key == message.Id.ToString()))
            {
                IConfiguration reactionMessage = reactionMessages.GetSection(message.Id.ToString());

                if (reactionMessage.GetChildren().Any(item => item.Key == reaction.Emote.Name))
                {
                    SocketGuild socketGuild = DiscordBot.Client.GetGuild(DiscordBot.GuildId);

                    IRole reactionRole = socketGuild.GetRole(Convert.ToUInt64(reactionMessage[reaction.Emote.Name]));
                    SocketGuildUser user = socketGuild.GetUser(reaction.UserId);
                    user.AddRoleAsync(reactionRole);
                }
            }

            return Task.CompletedTask;
        }

        public static Task ReactionRemovedHandler(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel,
            SocketReaction reaction)
        {
            IConfiguration reactionMessages = DiscordBot.Config.GetSection("ReactionMessages");

            if (reactionMessages.GetChildren().Any(item => item.Key == message.Id.ToString()))
            {
                IConfiguration reactionMessage = reactionMessages.GetSection(message.Id.ToString());

                if (reactionMessage.GetChildren().Any(item => item.Key == reaction.Emote.Name))
                {
                    SocketGuild socketGuild = DiscordBot.Client.GetGuild(DiscordBot.GuildId);

                    IRole reactionRole = socketGuild.GetRole(Convert.ToUInt64(reactionMessage[reaction.Emote.Name]));
                    SocketGuildUser user = socketGuild.GetUser(reaction.UserId);
                    user.RemoveRoleAsync(reactionRole);
                }
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Command Event Handlers
        public static Task LogExecutedCommand(SocketCommandContext context, SocketUserMessage message)
        {
            try
            {
                SocketTextChannel logChannel = LogChannels.Commands(context.Guild.Id);
                if (logChannel == null)
                {
                    return Task.CompletedTask;
                }
                

                Embed embed = new ExecutedCommand(context, message).Build();

                logChannel.SendMessageAsync(embed: embed);
            }
            catch (Exception e)
            {
                LogException(e, context.Guild.Id);
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}