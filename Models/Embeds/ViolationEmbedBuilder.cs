using System;
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
            
            WithTitle(Enum.GetName(typeof(ViolationTypes), violation.Type));
            Color = Discord.Color.Red;
            WithAuthor(author.ToString());
            AddField("User:", "<@!" + violation.UserId + ">", true);
            AddField("Date:", violation.Date, true);
            AddField("Moderator:", "<@!" + violation.ModeratorId + ">");
            AddField("Reason:", violation.Reason);
            AddField("Violation ID:", violation.Id, true);
            WithFooter("UserID: " + violation.UserId);
        }

    }
}