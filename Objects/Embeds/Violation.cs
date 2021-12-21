using Discord;
using Discord.WebSocket;
using DiscordBot.Class;

namespace DiscordBot.Objects.Embeds
{
    public class ViolationEmbedBuilder :EmbedBuilder
    {
        #region Constructors
        public ViolationEmbedBuilder(Violation violation, DiscordShardedClient client)
        {
            SetTitle(violation.Type);
            WithColor(Discord.Color.Red);
            //TODO Fix IUser is null error in EmbedAuthor constructor
            // WithAuthor(new EmbedAuthor(Rest.GetUserFromGuild(violation.Moderator, violation.Guild, client)));
            AddField("User:", "<@!" + violation.User + ">", true);
            AddField("Date:", violation.Date, true);
            AddField("Moderator:", "<@!" + violation.Moderator + ">");
            AddField("Reason:", violation.Reason);
            AddField("Violation ID:", violation.ViolationId, true);
            WithCurrentTimestamp();
            WithFooter("UserID: " + violation.User);
        }
        #endregion
        
        #region Methods
        private void SetTitle(int type)
        {
            switch (type)
            {
                case 1:
                    WithTitle("Banned");
                    break;
                case 2:
                    WithTitle("Kicked");
                    break;
                case 3:
                    WithTitle("Muted");
                    break;
                case 4:
                    WithTitle("Unmuted");
                    break;
                default:
                    WithTitle("Warned");
                    break;
            }
            
        }
        #endregion
    }
}