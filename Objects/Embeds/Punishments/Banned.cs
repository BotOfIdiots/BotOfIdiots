using Discord;
using Discord.WebSocket;

namespace DiscordBot.Objects.Embeds.Punishments
{
    public class Banned : EmbedBuilder
    {
        public Banned(SocketUser user)
        {
            WithTitle("User Banned");
            WithAuthor(new EmbedAuthor(user));
            AddField("user", user.Mention);
            WithCurrentTimestamp();
            WithColor(Discord.Color.Red);
            WithFooter("UserID: " + user.Id);
        }
    }
}