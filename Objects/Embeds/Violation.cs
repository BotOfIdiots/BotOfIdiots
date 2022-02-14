using System;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Class;

namespace DiscordBot.Objects.Embeds
{
    public class ViolationEmbedBuilder : EmbedBuilder
    {
        #region Constructors
        public ViolationEmbedBuilder(Violation violation, DiscordShardedClient client)
        {
            SetTitle(violation.Type);
            SocketGuildUser moderator = Rest.GetUserFromGuild(violation.Moderator, violation.Guild, client);

            BuildEmbedBody(violation.User, moderator, violation.Reason);
            AddField("Violation ID:", violation.ViolationId, true);
        }
        
        public ViolationEmbedBuilder(ulong user, SocketGuildUser moderator, string reason )
        {
            WithTitle("Unbanned");
            BuildEmbedBody(user, moderator, reason);
        }
        
        #endregion
        
        #region Methods

        private void BuildEmbedBody(ulong user, SocketGuildUser moderator, string reason)
        {
            WithColor(Discord.Color.Red);
            WithAuthor(new EmbedAuthor(moderator));
            WithCurrentTimestamp();
            WithFooter("UserID: " + user);
            
            AddField("User:", $"<@{user}>", true);
            AddField("Date:", DateTime.Now, true);
            AddField("Moderator:", moderator.Mention);
            AddField("Reason:", reason);
            
            
        }
        
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