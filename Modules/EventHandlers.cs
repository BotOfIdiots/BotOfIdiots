using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Models;
using DiscordBot.Models.Embeds;

namespace DiscordBot.Modules
{
    public static class EventHandlers
    {
        private static readonly LogChannels _logChannels = new LogChannels(DiscordBot.Config.GetSection("LogChannels"));
        
        public static Task LogException(Exception exception)
        {
            try
            {
                Embed exceptionEmbed = new EmbedBuilder
                    {
                        Title = exception.Message
                    }
                    .WithColor(Color.Red)
                    .WithDescription(exception.StackTrace)
                    .AddField("Source", exception.Source)
                    .WithFooter(DiscordBot.Version())
                    .WithCurrentTimestamp()
                    .Build();
                
                _logChannels.Exceptions.SendMessageAsync(embed: exceptionEmbed);
                Console.WriteLine(exception.ToString());
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return Task.CompletedTask;
            }
        }

        public static Task MessageDeleteHandler(Cacheable<IMessage, ulong> cachedMessage,
            ISocketMessageChannel channel)
        {
            try
            {
                if (cachedMessage.HasValue)
                {
                    var message = cachedMessage.Value;

                    Embed messageDeleteEmbed = new EmbedBuilder
                        {
                            Title = "Message Deleted"
                        }
                        .WithAuthor(message.Author)
                        .WithColor(Color.Red)
                        .AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")")
                        .AddField("Content", message.Content)
                        .AddField("Sent At", message.Timestamp.ToLocalTime()
                            .ToString("HH:mm:ss dd-MM-yyyy"))
                        .WithFooter("MessageID: " + message.Id)
                        .WithCurrentTimestamp()
                        .Build();

                    _logChannels.Messages.SendMessageAsync(embed: messageDeleteEmbed);
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

                    _logChannels.Messages.SendMessageAsync(embed: messageDeleteEmbed);
                    return Task.CompletedTask;
                }
                
                throw new Exception("Message Unhandled MessageDeleteHandler State");
            }
            catch (Exception e)
            {
                LogException(e);
                return Task.CompletedTask;
            }
        }

        public static Task MessageBulkDeleteHandler(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedData,
            ISocketMessageChannel channel)
        {
            try
            {
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
                    
                    _logChannels.Messages.SendMessageAsync(embed: messageBulkDeleteEmbed);
                    return Task.CompletedTask;
                }
                
                throw new Exception("Unhandled MessageBulkDeleteHandler state");
            }
            catch (Exception e)
            {
                LogException(e);
                return Task.CompletedTask;
            }
        }

        public static Task MessageUpdateHandler(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message,
            ISocketMessageChannel channel)
        {
            try
            {
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

                    _logChannels.Messages.SendMessageAsync(embed: messageUpdateEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MessageUpdateHandler State");
            }
            catch (Exception e)
            {
                LogException(e);
                return Task.CompletedTask;
            }
        }

        public static Task MemberJoinGuildHandler(SocketGuildUser joinedUser)
        {
            try
            {
                if (joinedUser != null)
                {
                    if (DiscordBot.Config["JoinRole"] != null)
                    {
                        IRole role = DiscordBot.Client.GetGuild(
                                DiscordBot.GuildId
                            ).GetRole(
                                Convert.ToUInt64(DiscordBot.Config["JoinRole"])
                                );
                        joinedUser.AddRoleAsync(role);
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

                    _logChannels.JoinLeave.SendMessageAsync(embed: memberJoinEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MemberJoinHandler state");
            }
            catch (Exception e)
            {
                LogException(e);
                return Task.CompletedTask;
            } 
        }
        
        public static Task MemberLeaveGuildHandler(SocketGuildUser leavingUser)
        {
            try
            {
                if (leavingUser != null)
                {
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

                    _logChannels.JoinLeave.SendMessageAsync(embed: memberJoinEmbed);
                    return Task.CompletedTask;
                }

                throw new Exception("Unhandled MemberJoinHandler state");
            }
            catch (Exception e)
            {
                LogException(e);
                return Task.CompletedTask;
            } 
        }

        public static Task MemberVoiceStateHandler(SocketUser user, SocketVoiceState stateBefore,
            SocketVoiceState stateAfter)
        {
            if (stateAfter.VoiceChannel == stateBefore.VoiceChannel)
            {
                return Task.CompletedTask;
            }
            
            try
            {
                Embed logEmbed = null;
                
                if (stateAfter.VoiceChannel == null) 
                {
                    logEmbed = new VoiceStateEmbedBuilder(0, user, stateBefore, stateAfter).Build();
                    
                }
                
                if (stateBefore.VoiceChannel == null )
                {
                    logEmbed = new VoiceStateEmbedBuilder(1, user, stateBefore, stateAfter).Build();
                }
                
                if (stateAfter.VoiceChannel != null && stateBefore.VoiceChannel != null)
                {
                    logEmbed = new VoiceStateEmbedBuilder(2, user, stateBefore, stateAfter).Build();
                }

                if (logEmbed != null)
                {
                    _logChannels.Voice.SendMessageAsync(embed: logEmbed);
                    return Task.CompletedTask;
                }
                
                throw new Exception("Unhandled Voice State");
            }
            catch (Exception e)
            {
                LogException(e);
                return  Task.CompletedTask;
            }
        }

        public static Task MemberUpdatedHandler(SocketGuildUser before, SocketGuildUser after)
        {
            try
            {
                if (before.Roles.Count != after.Roles.Count)
                {
                    _logChannels.Roles.SendMessageAsync(
                        embed: new MemberRolesUpdateEmbedBuilder(after, before.Roles.ToList()).Build()
                    );
                }

                if (before.Nickname != after.Nickname)
                {
                    _logChannels.Nickname.SendMessageAsync(
                        embed: new NicknameUpdateEmbedBuilder(after, before.Nickname).Build()
                    );
                }
            }
            catch (Exception e)
            {
                LogException(e);
            }
            
            return Task.CompletedTask;
        }

        public static Task MemberBannedHandler(SocketUser user, SocketGuild guild)
        {
            try
            {
                _logChannels.JoinLeave.SendMessageAsync(
                    embed: new EmbedBuilder()
                        .WithTitle("User Banned")
                        .AddField("user", user.Mention)
                        .WithCurrentTimestamp()
                        .WithColor(Color.Red)
                        .WithFooter("UserID: " + user.Id)
                        .Build()
                    );

                _logChannels.Logs.SendMessageAsync(
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
                LogException(e);
                return Task.CompletedTask;
            }   
        }

        public static Task MemberUnbannedHandler(SocketUser user, SocketGuild guild)
        {
            try
            {
                _logChannels.Logs.SendMessageAsync(
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
                LogException(e);
                return Task.CompletedTask;
            }
           
        }
    }
}