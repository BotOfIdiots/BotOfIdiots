using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task ping()
        {
            await ReplyAsync("Pong");
        }
    }
}