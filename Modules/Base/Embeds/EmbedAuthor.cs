using Discord;

namespace DiscordBot.Modules.Base.Embeds
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