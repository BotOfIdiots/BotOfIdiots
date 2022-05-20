using Discord;
using Discord.Interactions;

namespace DiscordBot.Modules.Base.Embeds
{
    public class BotVersion : EmbedBuilder
    {
        public BotVersion(ShardedInteractionContext context)
        {
            Title = "Version: " + DiscordBot.Version();
            WithAuthor(new EmbedAuthor(context.Client.CurrentUser));
            WithFooter(DiscordBot.Version());
            WithCurrentTimestamp();
        }
    }
}