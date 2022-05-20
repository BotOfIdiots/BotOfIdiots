using Discord;

namespace DiscordBot.Modules.Chat.Embeds;

public class ReactionRoleMessageEmbed : EmbedBuilder
{
    public ReactionRoleMessageEmbed(string content)
    {
        WithDescription(content);
    }
}