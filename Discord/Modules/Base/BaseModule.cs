using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Discord.Modules.Base.Commands;

namespace DiscordBot.Discord.Modules.Base;

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
        // await InteractionService.AddModulesAsync(Assembly.GetExecutingAssembly(), Service);
        await InteractionService.AddModuleAsync(typeof(Commands.Commands), Service);
        await InteractionService.AddModuleAsync(typeof(Punishment), Service);
        await InteractionService.AddModuleAsync(typeof(Chat.Commands.Settings), Service);
        await InteractionService.AddModuleAsync(typeof(Logging.Commands.Settings), Service);
        
        CommandHandler = new CommandHandler(InteractionService, Client, Service);
        await CommandHandler.RegisterCommandsAsync();
        await CommandHandler.Initialize();
    }

    private InteractionServiceConfig BuildServiceConfig()
    {
        return new InteractionServiceConfig();
    }
}