using Discord;

namespace DiscordBot.Objects
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