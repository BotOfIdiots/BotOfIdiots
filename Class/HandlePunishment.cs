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
    public static DatabaseService DatabaseService { get; set; }
    public static DiscordShardedClient Client { get; set; }
    
    public static Embed Warn(IDiscordClient client, DatabaseService databaseService, SocketGuildUser moderator,
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
            embed = ViolationManager.NewViolation(target, moderator, reason, Client, DatabaseService);

            SendMessageEmbedToUser(target, embed, Client, moderator.Guild).GetAwaiter();
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

    public static Embed Mute(SocketGuildUser moderator,
        SocketGuildUser target, string length, string reason)
    {
        Embed embed;

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

        if (target.TimedOutUntil > DateTimeOffset.Now)
        {
            embed = new EmbedBuilder
            {
                Title = "User is already muted"
            }.Build();

            return embed;
        }

        TimeSpan timeOutLength;

        string timeUnit = "";
        string duration = "";
        
        switch (timeUnit)
        {
            case "m":
                timeOutLength = TimeSpan.FromMinutes(Convert.ToDouble(duration));
                break;
            case "h":
                timeOutLength = TimeSpan.FromHours(Convert.ToDouble(duration));
                break;
            default:
                timeOutLength = TimeSpan.FromMinutes(5);
                break;
        }
        
        
        try
        {
            target.SetTimeOutAsync(timeOutLength);
            embed = ViolationManager.NewViolation(target, moderator, reason, Client, DatabaseService, 3);
            SendMessageEmbedToUser(target, embed, Client, target.Guild).GetAwaiter();
            LogViolation(embed, target.Guild).GetAwaiter();
        }
        catch (Exception exception)
        {
            embed = new EmbedBuilder().WithDescription("Couldn't mute user").Build();
        }
        
        return embed;
    }

    public static Embed Unmute(IDiscordClient client, DatabaseService databaseService, SocketGuildUser moderator,
        SocketGuildUser target, string reason)
    {
        Embed embed;

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

        if (target.TimedOutUntil <= DateTimeOffset.Now)
        {
            embed = new EmbedBuilder
            {
                Title = "User was not muted"
            }.Build();
            return embed;
        }

        target.RemoveTimeOutAsync();
        embed = ViolationManager.NewViolation(target, moderator, reason, Client, DatabaseService, 4);
        SendMessageEmbedToUser(target, embed, Client, target.Guild).GetAwaiter();
        LogViolation(embed, target.Guild).GetAwaiter();
        


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