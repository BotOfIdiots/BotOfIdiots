using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Modules.Logging.Embeds.Member
{
    public class MemberRolesUpdateEmbedBuilder : EmbedBuilder
    {
        #region Constructors
        public MemberRolesUpdateEmbedBuilder(SocketGuildUser user, List<SocketRole> rolesBefore)
        {
            WithTitle("Role list updated");
            WithDescription("Role list updated for " + user.Username + "#" + user.Discriminator);
            AddField("User", user.Mention);
            
            _rolesBefore(rolesBefore);
            _rolesAfter(user.Roles.ToList());

            WithColor(Discord.Color.Blue);
            WithCurrentTimestamp();
            WithFooter("UserID: " + user.Id);


        }
        #endregion
        
        #region Methods
        private void _rolesBefore(List<SocketRole> roles)
        {
            string roleString = null;
            foreach(SocketRole role in roles)
            {
                roleString += role.Mention;
            }

            AddField("Before", roleString);
        }
        
        private void _rolesAfter(List<SocketRole> roles)
        {
            string roleString = null;
            foreach(SocketRole role in roles)
            {
                roleString += role.Mention;
            }

            AddField("After", roleString);
        }
        #endregion
    }
}