using Discord;
using Discord.Interactions;

namespace DiscordBot.Discord.Modules.Base.Embeds
{
    public class BotVersion : EmbedBuilder
    {
        public BotVersion(ShardedInteractionContext context)
        {
            Title = "Version: " ; //Todo: import version from main class
            WithAuthor(new EmbedAuthor(context.Client.CurrentUser));
            WithFooter("Todo versiotn");
            WithCurrentTimestamp();
        }
    }
}