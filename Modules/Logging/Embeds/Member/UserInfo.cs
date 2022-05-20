using System;
using Discord;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Modules.Base.Class;
using DiscordBot.Modules.ViolationManagement;

namespace DiscordBot.Modules.Logging.Embeds.Member
{
    public class UserInfo : EmbedBuilder
    {
        private DatabaseService _databaseService;
        private DiscordShardedClient _client;
        private SocketGuildUser _user;
        private bool _moderation;
        
        public UserInfo(SocketGuildUser user, DiscordShardedClient client, DatabaseService databaseService,
            Boolean moderation = false)
        {
            _client = client;
            _databaseService = databaseService;
            _user = user;
            _moderation = moderation;
            
            BuildEmbed();
        }

        private void BuildEmbed()
        {
            WithAuthor(_client.CurrentUser.Username);
            WithThumbnailUrl(_user.GetAvatarUrl());
            AddField("User", _user.Mention);

            if (_moderation)
            {
                AddField("Violation Count:",
                    ViolationManager.CountUserViolations(_user.Id, _user.Guild.Id, _databaseService, _client));
            }

            AddField("Created At", _user.CreatedAt.ToString("dd-MM-yy HH:mm:ss"), true);
            AddField("Joined At", _user.JoinedAt?.ToString("dd-MM-yy HH:mm:ss"), true);
            AddField("Roles", Rest.CreateRolesList(_user.Roles));

            WithFooter("UserID: " + _user.Id);
            WithCurrentTimestamp();
        }
    }
}