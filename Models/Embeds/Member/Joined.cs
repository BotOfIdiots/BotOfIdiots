using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models.Embeds
{
    public class Joined : EmbedBuilder
    {
        public Joined(SocketGuildUser user)
        {
            Title = "Member Joined Server";

            WithAuthor(new EmbedAuthor(user));
            AddField("Username", user.Mention);
            AddField("User Created At", user.CreatedAt.ToLocalTime()
                .ToString("HH:mm:ss dd-MM-yyyy"));
            AddField("User Joined at", user.JoinedAt?.ToLocalTime()
                .ToString("HH:mm:ss dd-MM-yyyy"));
            WithColor(Discord.Color.Green);
            WithCurrentTimestamp();
            WithFooter("UserID: " + user.Id);

        }
        
    }
}