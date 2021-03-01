using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models.Embeds
{
    public class NicknameUpdateEmbedBuilder : EmbedBuilder
    {
        public NicknameUpdateEmbedBuilder(SocketGuildUser user, string OldNickname)
        {
            WithTitle("User Nickname updated");
            WithDescription("Nickname updated for " + user.Username + "#" + user.Discriminator);
            AddField("User", user.Mention);

            _oldNickname(OldNickname);
            _newNickname(user.Nickname);
            
            WithColor(Discord.Color.Blue);
            WithCurrentTimestamp();
            WithFooter("UserID: " + user.Id);
        }

        private void _oldNickname(string nickname = null)
        {
            if (nickname == null)
            {
                nickname = "(none)";
            }
            
            AddField("Old Nickname", nickname);
        }
        
        private void _newNickname(string nickname = null)
        {
            if (nickname == null)
            {
                nickname = "(none)";
            }
            
            AddField("New Nickname", nickname);
        }

    }
}