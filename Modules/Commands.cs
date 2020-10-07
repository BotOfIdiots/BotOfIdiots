using System;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }

        [Command("version")]
        public async Task Version()
        {
            Embed embed = new EmbedBuilder
            {
                Title = "Version: " + Program.Version(),
            }
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(Program.Version())
                .WithCurrentTimestamp()
                .Build();
            
            await ReplyAsync(embed: embed);
        }
    }
}