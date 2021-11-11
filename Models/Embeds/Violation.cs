using System;
using Discord;
using DiscordBot.Modules.Commands;

namespace DiscordBot.Models.Embeds
{
    public class ViolationEmbedBuilder :EmbedBuilder
    {
        #region Constructors
        public ViolationEmbedBuilder(Violation violation)
        {
            SetTitle(violation.Type);
            WithColor(Discord.Color.Red);
            WithAuthor(new EmbedAuthor(Functions.GetUserFromGuild(violation.Moderator, violation.Guild)));
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