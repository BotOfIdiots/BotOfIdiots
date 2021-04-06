using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models.Embeds
{
    public class ViolationEmbedBuilder : EmbedBuilder
    { 
        /// <summary>
        /// Creates an embedbuilder with all the information of the specified violation
        /// </summary>
        /// <param name="violationId"> The violation for which to create an embed</param>
        /// <param name="author">The author of the embed</param>
        public ViolationEmbedBuilder(int violationId, SocketSelfUser author)
        {
            Violation violation = Violation.GetRecord(violationId);
            
            SetType(violation.Type);
            Color = Discord.Color.Red;
            WithAuthor(author.ToString());
            AddField("User:", "<@!" + violation.UserId + ">", true);
            AddField("Date:", violation.Date, true);
            AddField("Moderator:", "<@!" + violation.ModeratorId + ">");
            AddField("Reason:", violation.Reason);
            AddField("Violation ID:", violation.Id, true);
            WithFooter("UserID: " + violation.UserId);
        }

        /// <summary>
        /// Sets the title of the embed based on the violation type
        /// </summary>
        /// <param name="type">The type on which the title is based</param>
        private void SetType(int type)
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

    }
}