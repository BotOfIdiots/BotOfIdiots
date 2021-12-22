using System;
using System.Linq;
using System.Net;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Objects.Embeds;
using static DiscordBot.Class.Rest;
using static DiscordBot.Modules.EventHandlers;

namespace DiscordBot.Class;

public static class HandlePunishment
{
    public static Embed Warn(DiscordShardedClient client, DatabaseService databaseService, SocketGuildUser moderator,
        SocketGuildUser target, string reason)
    {
        Embed embed;
        if (target == moderator)
        {
            embed = new EmbedBuilder
            {
                Title = "You can't warn that user"
            }.Build();
        }
        else
        {
            embed = ViolationManager.NewViolation(target, moderator, reason, client, databaseService);

            SendMessageEmbedToUser(target, embed, client, moderator.Guild).GetAwaiter();
            LogViolation(embed, moderator.Guild);
        }

        return embed;
    }

    public static Embed Kick(DiscordShardedClient client, DatabaseService databaseService, SocketGuildUser moderator,
        SocketGuildUser target, string reason)
    {
        Embed embed;
        if (target == moderator)
        {
            embed = new EmbedBuilder
            {
                Title = "You can't kick that user"
            }.Build();
        }
        else
        {
            embed = ViolationManager.NewViolation(target, moderator, reason, client, databaseService, 2);

            SendMessageEmbedToUser(target, embed, client, target.Guild).GetAwaiter();
            LogViolation(embed, moderator.Guild).GetAwaiter();
            target.KickAsync(reason).GetAwaiter();
        }

        return embed;
    }

    public static Embed Mute(DiscordShardedClient client, DatabaseService databaseService, SocketGuildUser moderator,
        SocketGuildUser target, string reason)
    {
        Embed embed;

        SocketRole mutedRole = DbOperations.GetMutedRole(target.Guild);

        if (mutedRole == null)
        {
            embed = new EmbedBuilder
            {
                Description = "Muted Role not defined"
            }.Build();
            return embed;
        }

        if (target == moderator)
        {
            embed = new EmbedBuilder
            {
                Description = "You can't mute that user."
            }
                .WithColor(Color.Red)
                .Build();

            return embed;
        }

        if (target.Roles.Contains(mutedRole))
        {
            embed = new EmbedBuilder
            {
                Title = "User is already muted"
            }.Build();

            return embed;
        }


        embed = ViolationManager.NewViolation(target, moderator, reason, client, databaseService, 3);

        SendMessageEmbedToUser(target, embed, client, target.Guild).GetAwaiter();
        LogViolation(embed, target.Guild).GetAwaiter();
        target.AddRoleAsync(mutedRole).GetAwaiter();

        return embed;
    }

    public static Embed Unmute(DiscordShardedClient client, DatabaseService databaseService, SocketGuildUser moderator,
        SocketGuildUser target, string reason)
    {
        Embed embed;

        SocketRole mutedRole = DbOperations.GetMutedRole(target.Guild);

        if (mutedRole == null)
        {
            embed = new EmbedBuilder
            {
                Description = "Muted Role not defined"
            }.Build();
            return embed;
        }

        if (target == moderator)
        {
            embed = new EmbedBuilder
            {
                Description = "You can't unmute that user."
            }
                .WithColor(Color.Red)
                .Build();
            return embed;
        }

        if (!target.Roles.Contains(mutedRole))
        {
            embed = new EmbedBuilder
            {
                Title = "User was not muted"
            }.Build();
            return embed;
        }

        embed = ViolationManager.NewViolation(target, moderator, reason, client, databaseService, 4);

        SendMessageEmbedToUser(target, embed, client, target.Guild).GetAwaiter();
        LogViolation(embed, target.Guild).GetAwaiter();
        target.RemoveRoleAsync(mutedRole).GetAwaiter();


        return embed;
    }


    public static Embed Ban(DiscordShardedClient client, DatabaseService databaseService, SocketGuildUser moderator,
        SocketGuildUser target, string reason)
    {
        Embed embed;
        if (target == moderator)
        {
            embed = new EmbedBuilder
                {
                    Title = "You can't ban that user"
                }
                .Build();
        }
        else
        {
            embed = ViolationManager.NewViolation(target, moderator, reason, client, databaseService, 1);

            SendMessageEmbedToUser(target, embed, client, target.Guild).GetAwaiter();
            LogViolation(embed, target.Guild).GetAwaiter();
            target.BanAsync(1, reason).GetAwaiter();
        }

        return embed;
    }

    public static Embed Unban(DiscordShardedClient client, DatabaseService databaseService,
        SocketGuildUser moderator,
        ulong target, string reason, SocketGuild guild)

    {
        Embed embed;
        
        try
        {
            embed = new ViolationEmbedBuilder(target, moderator, reason).Build();
            
            guild.RemoveBanAsync(target).GetAwaiter();
            LogViolation(embed, moderator.Guild).GetAwaiter();
            return embed;
        }
        catch (HttpException e)
        {
            if (e.HttpCode == HttpStatusCode.NotFound)
            {
                embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithDescription("Banned User not found")
                    .Build();
            }
            else
            {
                embed = new EmbedBuilder()
                    .WithColor(Color.Red)
                    .WithDescription("Couldn't unban user")
                    .Build();
            }

            LogException(e, guild);
            
        }

        return embed;
    }
}