using Discord;
using Discord.WebSocket;
using DiscordBot.Modules.Base.Class;

namespace DiscordBot.Modules.Base.Embeds;

public class ServerInfo : EmbedBuilder
{
    public ServerInfo(SocketGuild guild)
    {
        WithTitle(guild.Name);
        WithDescription(guild.Description);
        WithThumbnailUrl(guild.IconUrl);
        AddField("Owner", guild.Owner.Mention, true);
        // AddField("Connected Shard", "Shard #" + DiscordBot.ShardedClient.GetShardIdFor(guild), true);
        AddField("Member Count", guild.Users.Count, true);
        AddField("Created At", guild.CreatedAt.ToString("dd-MM-yy HH:mm:ss"));
        AddField("Roles(" + guild.Roles.Count + ")", Rest.CreateRolesList(guild.Roles));
        WithFooter("GuildID: " + guild.Id);
        WithCurrentTimestamp();
    }
}