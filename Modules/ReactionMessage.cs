using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Objects.Embeds.Config;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Modules
{
    [Group("reactionmessage")]
    public class ReactionMessage : ModuleBase<SocketCommandContext>
    {
        #region Reaction Hanlder
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
        #endregion

        //TODO Implement Reaction Message setup Commands
        [Command("set message")]
        public async Task SetMessage(IMessage message)
        {
            if (DbOperations.InsertReactionMessage(message.Id, Context.Guild))
            {
                Embed reactionMessageConfig = new ReactionMessageConfig(message).Build();
                await ReplyAsync(embed: reactionMessageConfig);
            }
            await Task.CompletedTask;
        }

        [Command("remove message")]
        public async Task RemoveMessage()
        {
            throw new NotImplementedException();
        }
    }
}