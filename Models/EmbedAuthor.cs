using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models
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