using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Modules.Base.Commands;

namespace DiscordBot.Modules.Base;

public class BaseModule
{
    protected readonly IServiceProvider Service;
    protected readonly DiscordShardedClient Client;
    protected readonly InteractionService InteractionService;
    public CommandHandler CommandHandler;
    
    public BaseModule(DiscordShardedClient client, IServiceProvider service)
    {
        InteractionServiceConfig config = BuildServiceConfig();
        
        Service = service;
        Client = client;
        InteractionService = new InteractionService(Client);
    }
    
    public async Task Initialize()
    {
        await InteractionService.AddModuleAsync(typeof(Commands.Commands), Service);
        await InteractionService.AddModuleAsync(typeof(Punishment), Service);
        
        CommandHandler = new CommandHandler(InteractionService, Client, Service);
        await CommandHandler.RegisterCommandsAsync();
        await CommandHandler.Initialize();
    }

    private InteractionServiceConfig BuildServiceConfig()
    {
        return new InteractionServiceConfig();
    }
}