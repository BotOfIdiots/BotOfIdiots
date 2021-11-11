using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Modules
{
    public class ReactionMessage
    {
        public static Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            SocketTextChannel textChannel = (SocketTextChannel)channel;
            IConfiguration reactionMessages = DiscordBot.Config.GetSection("ReactionMessages");

            if (reactionMessages.GetChildren().Any(item => item.Key == message.Id.ToString()))
            {
                IConfiguration reactionMessage = reactionMessages.GetSection(message.Id.ToString());

                if (reactionMessage.GetChildren().Any(item => item.Key == reaction.Emote.Name))
                {
                    SocketGuild socketGuild = textChannel.Guild;

                    IRole reactionRole = socketGuild.GetRole(Convert.ToUInt64(reactionMessage[reaction.Emote.Name]));
                    SocketGuildUser user = socketGuild.GetUser(reaction.UserId);
                    user.AddRoleAsync(reactionRole);
                }
            }

            return Task.CompletedTask;
        }

        public static Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            SocketTextChannel textChannel = (SocketTextChannel)channel;

            IConfiguration reactionMessages = DiscordBot.Config.GetSection("ReactionMessages");

            if (reactionMessages.GetChildren().Any(item => item.Key == message.Id.ToString()))
            {
                IConfiguration reactionMessage = reactionMessages.GetSection(message.Id.ToString());

                if (reactionMessage.GetChildren().Any(item => item.Key == reaction.Emote.Name))
                {
                    SocketGuild socketGuild = textChannel.Guild;

                    IRole reactionRole = socketGuild.GetRole(Convert.ToUInt64(reactionMessage[reaction.Emote.Name]));
                    SocketGuildUser user = textChannel.Guild.GetUser(reaction.UserId);
                    user.RemoveRoleAsync(reactionRole);
                }
            }
            return Task.CompletedTask;
        }
    }
}