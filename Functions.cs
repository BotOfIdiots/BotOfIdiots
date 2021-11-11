using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot
{
    public static class Functions
    {
        public static async Task SendMessageEmbedToUser(SocketGuildUser user, Embed embed, SocketCommandContext context)
        {
            try
            {
                await user.SendMessageAsync(embed: embed);
            }
            catch (Exception)
            {
                embed = new EmbedBuilder
                {
                    Title = "Cannot Send Message To user"
                }
                    .WithDescription("Could not send embed with information pertaining to this action, to the user in question")
                    .WithAuthor(context.Client.CurrentUser)
                    .WithColor(Color.Red)
                    .WithCurrentTimestamp()
                    .Build();

                SocketTextChannel replyChannel = context.Guild.GetTextChannel(context.Channel.Id);
                await replyChannel.SendMessageAsync(embed: embed);
            }
        }

        public static IUser GetUserFromGuild(ulong user, ulong guild)
        {
            return DiscordBot.Client.GetGuild(guild).GetUser(user);
        }
    }
}