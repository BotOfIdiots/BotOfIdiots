using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Objects.Embeds.Member
{
    public class UserInfo : EmbedBuilder
    {
        public UserInfo(SocketGuildUser user, Boolean moderation = false)
        {
            WithAuthor(DiscordBot.ShardedClient.CurrentUser.Username);
            WithThumbnailUrl(user.GetAvatarUrl());
            AddField("User", user.Mention);
            
            if (moderation)
            {
                AddField("Violation Count:", ViolationManager.CountUserViolations(user.Id, user.Guild.Id));
            }
            
            AddField("Created At", user.CreatedAt.ToString("dd-MM-yy HH:mm:ss"), true);
            AddField("Joined At", user.JoinedAt?.ToString("dd-MM-yy HH:mm:ss"), true);
            AddField("Roles", Functions.CreateRolesList(user.Roles));
           
            WithFooter("UserID: " + user.Id);
            WithCurrentTimestamp();
        }
        
    }
}