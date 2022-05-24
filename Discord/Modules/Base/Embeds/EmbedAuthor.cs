using Discord;

namespace DiscordBot.Discord.Modules.Base.Embeds
{
    public class EmbedAuthor : EmbedAuthorBuilder
    {
        public EmbedAuthor(IUser user)
        {
            Name = user.Username + "#" + user.DiscriminatorValue;
            IconUrl = user.GetAvatarUrl();
        }
    }
}