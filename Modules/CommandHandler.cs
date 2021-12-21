using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Modules.Commands.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using static DiscordBot.Class.HandlePunishment;
using DiscordBot.Class;

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
    /// <param name="command">The slash command to be executed</param>
    public async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        string commandName = command.Data.Name;

        switch (commandName)
        {
            case "warn":
                await HandleWarnCommand(command);
                break;
        }

        await command.RespondAsync($"You executed {command.Data.Name}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public async Task HandleCommandAsync(SocketMessage arg)
    {
        CommandService Command = DiscordBot.Services.GetRequiredService<CommandService>();


        var message = arg as SocketUserMessage;
        ShardedCommandContext context =
            new ShardedCommandContext(_shardedClient, message);
        if (message.Author.IsBot) return;

        int argPos = 0;
        if (message.HasStringPrefix("$", ref argPos))
        {
            var result = await Command.ExecuteAsync(context, argPos, DiscordBot.Services);

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

    public async Task HandleWarnCommand(SocketSlashCommand command)
    {
        SocketSlashCommandData data = command.Data;

        var (user, reason ) = data.Options;
        SocketGuildUser moderator = (SocketGuildUser)command.User;

        await command.RespondAsync(embed: Warn(_shardedClient, _databaseService, moderator, (SocketGuildUser)user, (String)reason));
    }

    public async Task RegisterCommandsAsync()
    {
        try
        {
            await _shardedClient.Rest.BulkOverwriteGuildCommands(CommandCollection(), 317226837841281024);
            await _shardedClient.Rest.DeleteAllGlobalCommandsAsync();
            // await _shardedClient.Rest.BulkOverwriteGlobalCommands(CommandCollection());
        }
        catch (ApplicationCommandException ex)
        {
            Console.WriteLine(ex);
        }

        await Task.CompletedTask;
    }

    private ApplicationCommandProperties[] CommandCollection()
    {
        ApplicationCommandProperties[] commandCollection = 
        {
            new Warn().Build(),
        };

        return commandCollection;
    }
}