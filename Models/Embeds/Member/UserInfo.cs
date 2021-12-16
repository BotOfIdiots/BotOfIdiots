using System;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models.Embeds
{
    public class UserInfo : EmbedBuilder
    {
        public UserInfo(SocketGuildUser user, Boolean moderation = false)
        {
            String roles = null;
            foreach (IRole role in user.Roles.Distinct())
            {
                if (roles == null)
                {
                    roles = role.Mention;
                }
                else
                {
                    roles += role.Mention;
                }
            }

            AddField("User", user.Mention);
            WithThumbnailUrl(user.GetAvatarUrl());

            if (moderation)
            {
                AddField("Violation Count:", ViolationManager.CountUserViolations(user.Id, user.Guild.Id));
            }
            AddField("Created At", user.CreatedAt.ToString("dd-MM-yy HH:mm:ss"), true);
            AddField("Joined At", user.JoinedAt?.ToString("dd-MM-yy HH:mm:ss"), true);
            AddField("Roles", roles);
            WithAuthor(DiscordBot.Client.CurrentUser.Username);
            WithFooter("UserID: " + user.Id);
            WithCurrentTimestamp();
        }
    }
}