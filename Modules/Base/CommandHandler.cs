using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;

namespace DiscordBot.Modules.Base;

public class CommandHandler
{
    private readonly InteractionService _commands;
    private readonly DiscordShardedClient _client;
    private readonly IServiceProvider _services;

    public CommandHandler(InteractionService commands, DiscordShardedClient client, IServiceProvider services)
    {
        _commands = commands;
        _client = client;
        _services = services;
    }

    // 317226837841281024

    public async Task RegisterCommandsAsync()
    {
        try
        {
#if DEBUG
            await _commands.RegisterCommandsToGuildAsync(317226837841281024);
            foreach (var command in _commands.SlashCommands)
            {
                Console.WriteLine(command.Name + ": " + command.Description);   
            }
#else
            await _commands.RegisterCommandsGloballyAsync();
#endif
        }
        catch (HttpException ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task RegisterCommandsAsync(ulong guildId)
    {
        try
        {
            await _commands.RegisterCommandsToGuildAsync(guildId);
            foreach (var command in _commands.SlashCommands)
            {
                Console.WriteLine(command.Name + ": " + command.Description);   
            }
        }
        catch (HttpException ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task Initialize()
    {
        try
        {
            _client.InteractionCreated += InteractionCreated;
            _client.ShardReady += Ready;
            _commands.SlashCommandExecuted += _commands_SlashCommandExecuted;
            _commands.AutocompleteHandlerExecuted += _commands_AutocompleteHandlerExecuted;


            _commands.Log += (msg) =>
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            };
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private async Task InteractionCreated(SocketInteraction interaction)
    {
        try
        {
            var ctx = new ShardedInteractionContext(_client, interaction);
            var result = await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }


    private async Task Ready(DiscordSocketClient arg)
    {
        await Task.CompletedTask;
        _client.ShardReady -= Ready;
    }


    private Task _commands_SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        return Task.CompletedTask;
    }

    private Task _commands_AutocompleteHandlerExecuted(IAutocompleteHandler arg1, IInteractionContext arg2,
        IResult arg3)
    {
        return Task.CompletedTask;
    }
}