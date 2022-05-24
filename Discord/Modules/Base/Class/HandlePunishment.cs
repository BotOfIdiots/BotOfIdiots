using System;
using System.Net;
using System.Text.RegularExpressions;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Discord.Modules.ViolationManagement;
using DiscordBot.Discord.Modules.ViolationManagement.Embeds;
using static DiscordBot.Discord.Modules.Base.Class.Rest;
using static DiscordBot.Discord.Modules.Event.EventHandlers;

namespace DiscordBot.Discord.Modules.Base.Class;

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
                Description = "You can't Time-out that user."
            }
                .WithColor(Color.Red)
                .Build();

            return embed;
        }

        if (target.TimedOutUntil > DateTimeOffset.Now)
        {
            embed = new EmbedBuilder
            {
                Title = "User is already timed-out"
            }.Build();

            return embed;
        }

        TimeSpan timeOutLength;

        string timeUnit = length.Substring(length.Length - 1);
        Double duration = Convert.ToDouble(Regex.Match(length, @"\d+").Value);
        
        switch (timeUnit)
        {
            case "m":
                timeOutLength = TimeSpan.FromMinutes(duration);
                break;
            case "h":
                timeOutLength = TimeSpan.FromHours(duration);
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
        catch (Exception)
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