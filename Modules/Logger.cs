using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Models;

namespace DiscordBot.Modules
{
    public static class Logger
    {
        private static LogChannels _logChannels = new LogChannels(DiscordBot.Config.GetSection("LogChannels"));
        
        public static Task LogException(Exception exception)
        {
            try
            {
                Embed exceptionEmbed = new EmbedBuilder
                    {
                        Title = exception.Message
                    }
                    .WithColor(Color.Red)
                    .AddField("Source", exception.Source)
                    .AddField("Exception",  exception.StackTrace)
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
                        .AddField("Sent At", message.Timestamp.ToLocalTime().ToString("dd-MM-yy HH:mm:ss"))
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

        public static Task MessageBulkDeleteHandler(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedData, ISocketMessageChannel channel)
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
                        .AddField("Sent At", message.Timestamp.ToLocalTime().ToString("dd-MM-yy HH:mm:ss"))
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

        public static Task MemberJoinHandler(SocketGuildUser joinedUser)
        {
            try
            {
                if (joinedUser != null)
                {
                    Embed memberJoinEmbed = new EmbedBuilder
                        {
                            Title = "Member Joined"
                        }
                        .WithAuthor(joinedUser)
                        .AddField("Username", joinedUser.Username)
                        .AddField("User Created At", joinedUser.CreatedAt.ToLocalTime().ToString("dd-MM-yy HH:mm:ss"))
                        .AddField("User Joined at", joinedUser.JoinedAt?.ToLocalTime().ToString("dd-MM-yy HH:mm:ss"))
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
        
        public static Task MemberLeaveHandler(SocketGuildUser leavingUser)
        {
            try
            {
                if (leavingUser != null)
                {
                    Embed memberJoinEmbed = new EmbedBuilder
                        {
                            Title = "Member Left"
                        }
                        .WithAuthor(leavingUser)
                        .AddField("Username", leavingUser.Username)
                        .AddField("User Created At", leavingUser.CreatedAt.ToLocalTime().ToString("dd-MM-yy HH:mm:ss"))
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
    }
}