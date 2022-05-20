using Discord;
using Discord.WebSocket;
using EmbedAuthor = DiscordBot.Modules.Base.Embeds.EmbedAuthor;

namespace DiscordBot.Modules.Logging.Embeds.Member
{
    public class JoinLeave : EmbedBuilder
    {
        public JoinLeave(SocketGuildUser user, bool leave)
        {


            AddField("Username", user.Mention);
            AddField("User Created At", user.CreatedAt.ToLocalTime()
                .ToString("HH:mm:ss dd-MM-yyyy"));
            IsLeave(leave, user);
            
            WithAuthor(new EmbedAuthor(user));
            WithCurrentTimestamp();
            WithFooter("UserID: " + user.Id);

        }

        private void IsLeave(bool leave, SocketGuildUser user)
        {
            if (leave)
            {
                Title = "Member Left Server";
                WithColor(Discord.Color.Red);
            }
            else
            {
                Title = "Member Joined Server";
                AddField("User Joined at", user.JoinedAt?.ToLocalTime()
                    .ToString("HH:mm:ss dd-MM-yyyy"));
                WithColor(Discord.Color.Green);
            }
            
        }
        
        
    }
}