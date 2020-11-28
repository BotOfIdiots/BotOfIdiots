using System;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public static class Logger
    {
        static SocketTextChannel messageChannel = DiscordBot.Client.GetGuild(DiscordBot.GuildId)
            .GetTextChannel(DiscordBot.LogChannels.Messages);

        static SocketTextChannel exceptionChannel = DiscordBot.Client.GetGuild(DiscordBot.GuildId)
            .GetTextChannel(DiscordBot.LogChannels.Exceptions);

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
                    .AddField("Exception", exception.StackTrace)
                    .WithFooter(DiscordBot.Version())
                    .WithCurrentTimestamp()
                    .Build();

                exceptionChannel.SendMessageAsync(embed: exceptionEmbed);
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

                    messageChannel.SendMessageAsync(embed: messageDeleteEmbed);
                    return Task.CompletedTask;
                }
                if (!cachedMessage.HasValue)
                {
                    Embed messageDeleteEmbed = new EmbedBuilder
                        {
                            Title = "Message Deleted"
                        }.WithColor(Color.Red)
                        .WithDescription("Could not retrieve message from cache")
                        .WithFooter("MessageID: " + cachedMessage.Id)
                        .WithCurrentTimestamp()
                        .Build();

                    messageChannel.SendMessageAsync(embed: messageDeleteEmbed);
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

                    messageChannel.SendMessageAsync(embed: messageUpdateEmbed);
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
    }
}