using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Modules.Base;

namespace DiscordBot.Modules.Logging;

public class LoggingModule: BaseModule
{
    public LoggingModule(DiscordShardedClient client, IServiceProvider services) : base(client, services)
    {
        Initialize().GetAwaiter();
    }

    private new async Task Initialize()
    {
        await InteractionService.AddModuleAsync(typeof(Task), Service);
        CommandHandler = new CommandHandler(InteractionService, Client, Service);
    }
    
}