using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models.Embeds.Punishments
{
    public class Unbanned : EmbedBuilder
    {
        public Unbanned(SocketUser user)
        {
            WithTitle("User Unbanned");
            AddField("user", user.Mention);
            WithCurrentTimestamp();
            WithColor(Discord.Color.Green);
            WithFooter("UserID: " + user.Id);
        }
    }
}