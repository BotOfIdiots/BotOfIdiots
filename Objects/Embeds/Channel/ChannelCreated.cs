using Discord;
using Discord.WebSocket;

namespace DiscordBot.Objects.Embeds.Channel
{
    public class ChannelCreatedEmbedBuilder : EmbedBuilder
    {
        #region Constructors

        public ChannelCreatedEmbedBuilder(SocketGuildChannel channel)
        {
            WithTitle("Channel Created");
            WithDescription("A new Channel has been added to the Guild");
            WithColor(Discord.Color.Green);
            AddField("Channel name: ", channel.Name);
            // AddPermissionOverwriteField(channel.PermissionOverwrites.ToList());
            WithCurrentTimestamp();
            WithFooter("ChannelId: " + channel.Id);
        }

        #endregion

        #region Methods
        // private void AddPermissionOverwriteField(List<Overwrite> permissionOverwrites)
        // {
        //     foreach (Overwrite overwrite in permissionOverwrites)
        //     {
        //         string fieldName = "";
        //         if (overwrite.TargetType.ToString() == "Role")
        //         {
        //             fieldName = "Role:" + DiscordBot.Client.GetGuild(DiscordBot.GuildId).GetRole(overwrite.TargetId).Name;
        //         }
        //
        //         if (overwrite.TargetType.ToString() == "User")
        //         {
        //             fieldName = "User: " + DiscordBot.Client.GetGuild(DiscordBot.GuildId).GetUser(overwrite.TargetId).Username;
        //         }
        //
        //         if (fieldName != "")
        //         {
        //             foreach (var channelPermission in overwrite.Permissions.ToAllowList())
        //             {
        //                 channelPermission.
        //             }
        //             
        //             AddField(fieldName, );
        //         }
        //     }
        // }
        #endregion
    }
}