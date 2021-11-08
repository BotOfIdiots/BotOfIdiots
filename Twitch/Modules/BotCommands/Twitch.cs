using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBot.Twitch.BotCommands
{
    [RequireUserPermission(GuildPermission.Administrator)]
    [Group("debug")]
    [Summary("Debugging for twitch module")]
    public class DebuggingModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task Debug()
        {
            Embed embed = new EmbedBuilder
                {
                    Title = "Twitch Debug mode"
                }
                .AddField("Enabled", Base.TwitchIntegration.Debug)
                .Build();

            await ReplyAsync(embed: embed);
        }
        
        
        [Command("set")]
        public async Task DebugSet(bool value)
        {
            Base.TwitchIntegration.Debug = value;
            
            Embed embed = new EmbedBuilder
                {
                    Title = "Twitch Debug mode"
                }
                .AddField("Enabled", Base.TwitchIntegration.Debug)
                .Build();
            
            await ReplyAsync(embed: embed);
        }
    }
}