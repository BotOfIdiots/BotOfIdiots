using Discord;
using Discord.WebSocket;

namespace DiscordBot.Class;

public static class PermissionChecks
{
    public static bool RequireGuildPermission(GuildPermission permission, SocketSlashCommand command)
    {
        if ((command.User as SocketGuildUser).GuildPermissions.Has(permission))
        {
            return true;
        }

        command.RespondAsync(
            embed: new EmbedBuilder().WithDescription("You don't have permission to use that command")
                .WithColor(Color.Red)
                .Build());
        
        return false;
    }
}