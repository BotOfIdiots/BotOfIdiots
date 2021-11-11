using System;
using Discord;

namespace DiscordBot.Models.Embeds
{
    public class BotVersion : EmbedBuilder
    {
        public BotVersion()
        {
            Title = "Version: " + DiscordBot.Version();
            WithAuthor(new EmbedAuthor(DiscordBot.Client.CurrentUser));
            WithFooter(DiscordBot.Version());
            WithCurrentTimestamp();
        }
    }
}