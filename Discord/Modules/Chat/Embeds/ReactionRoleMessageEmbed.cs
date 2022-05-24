using Discord;

namespace DiscordBot.Discord.Modules.Chat.Embeds;

public class ReactionRoleMessageEmbed : EmbedBuilder
{
    public ReactionRoleMessageEmbed(string content)
    {
        WithDescription(content);
    }
}