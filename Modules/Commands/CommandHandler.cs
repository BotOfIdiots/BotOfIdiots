using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Class;
using DiscordBot.Database;
using DiscordBot.Modules.Commands.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using static DiscordBot.Class.HandlePunishment;
using static DiscordBot.Class.PermissionChecks;

namespace DiscordBot.Modules;

public class CommandHandler
{
    private DiscordShardedClient _shardedClient;
    private DatabaseService _databaseService;

    public CommandHandler(DiscordShardedClient client, DatabaseService databaseService)
    {
        _shardedClient = client;
        _databaseService = databaseService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public async Task HandleCommandAsync(SocketMessage arg)
    {
        CommandService command = DiscordBot.Services.GetRequiredService<CommandService>();


        var message = arg as SocketUserMessage;
        ShardedCommandContext context =
            new ShardedCommandContext(_shardedClient, message);
        try
        {
            if (message.Author.IsBot) return;
            
            int argPos = 0;
            if (message.HasStringPrefix("$", ref argPos))
            {
                var result = await command.ExecuteAsync(context, argPos, DiscordBot.Services);

                if (!result.IsSuccess)
                {
                    Embed exceptionEmbed = new EmbedBuilder()
                        .WithColor(Color.Red)
                        .WithDescription(result.ErrorReason)
                        .Build();

                    await context.Channel.SendMessageAsync(embed: exceptionEmbed);
                }

                if (result.IsSuccess)
                {
                    await EventHandlers.LogExecutedCommand(context, message);
                }
            }
        }
        catch (NullReferenceException e)
        {
            await EventHandlers.LogException(e, context.Guild);
        }
        

        
    }

    public async Task RegisterCommandsAsync()
    {
        try
        {
            await _shardedClient.Rest.BulkOverwriteGlobalCommands(CommandCollection());
        }
        catch (HttpException ex)
        {
            Console.WriteLine(ex);
        }

        await Task.CompletedTask;
    }

    private ApplicationCommandProperties[] CommandCollection()
    {
        ApplicationCommandProperties[] commandCollection =
        {
            new WarnCommand().Build(),
            new MuteCommand().Build()
        };

        return commandCollection;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command">The slash command to be executed</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        string commandName = command.Data.Name;

        switch (commandName)
        {
            case "warn":
                await HandleWarnCommand(command);
                break;
            case "kick":
                break;
            case "mute":
                await HandleMuteCommand(command);
                break;
            case "ban":
                break;
            case "unban":
                break;
        }
    }


    async Task HandleWarnCommand(SocketSlashCommand command)
    {
        if (RequireGuildPermission(GuildPermission.KickMembers, command))
        {
            SocketGuildUser moderator = (SocketGuildUser)command.User;
            SocketSlashCommandData data = command.Data;
            var (user, reason) = data.Options;

            await command.RespondAsync(embed: Warn(_shardedClient, _databaseService, moderator, (SocketGuildUser)user,
                (String)reason));
        }
    }

    async Task HandleMuteCommand(SocketSlashCommand command)
    {
        if (RequireGuildPermission(GuildPermission.KickMembers, command))
        {
            SocketGuildUser moderator = (SocketGuildUser)command.User;
            SocketSlashCommandData data = command.Data;
            var (user, reason) = data.Options;

            await command.RespondAsync(embed: Mute(_shardedClient, _databaseService, moderator, (SocketGuildUser)user,
                (String)reason));
        }
    }
}