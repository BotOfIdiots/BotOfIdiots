using Discord;
using Discord.WebSocket;
using EmbedAuthor = DiscordBot.Discord.Modules.Base.Embeds.EmbedAuthor;

namespace DiscordBot.Discord.Modules.Logging.Embeds.Punishments
{
    public class Unbanned : EmbedBuilder
    {
        public Unbanned(SocketUser user)
        {
            WithTitle("User Unbanned");
            WithAuthor(new EmbedAuthor(user));
            AddField("user", user.Mention);
            WithCurrentTimestamp();
            WithColor(global::Discord.Color.Green);
            WithFooter("UserID: " + user.Id);
        }
    }
}