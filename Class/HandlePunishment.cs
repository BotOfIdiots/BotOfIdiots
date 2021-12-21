using Discord;
using Discord.WebSocket;
using DiscordBot.Database;
using static DiscordBot.Class.Rest;
using static DiscordBot.Modules.EventHandlers;

namespace DiscordBot.Class;

public class HandlePunishment
{
    public static Embed Warn(DiscordShardedClient client, DatabaseService databaseService, SocketGuildUser moderator,
        SocketGuildUser warnedUser, string reason)
    {
        Embed embed;
        if (warnedUser == moderator)
        {
            embed = new EmbedBuilder
            {
                Title = "You can't warn that user"
            }.Build();
        }
        else
        {
            embed = ViolationManager.NewViolation(warnedUser, moderator, reason, client, databaseService);

            SendMessageEmbedToUser(warnedUser, embed, client, moderator.Guild);
            LogViolation(embed, moderator.Guild);
        }

        return embed;
    }
}