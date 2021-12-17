using System;
using Discord;
using Discord.Commands;

namespace DiscordBot.Models.Embeds
{
    public class BotVersion : EmbedBuilder
    {
        public BotVersion(ShardedCommandContext context)
        {
            Title = "Version: " + DiscordBot.Version();
            WithAuthor(new EmbedAuthor(DiscordBot.ShardedClient.CurrentUser));
            WithFooter(DiscordBot.Version());
            WithCurrentTimestamp();
        }
    }
}