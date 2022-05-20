using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Modules.Base;
using DiscordBot.Modules.Logging.Commands;

namespace DiscordBot.Modules.Logging;

public class LoggingModule: BaseModule
{
    public LoggingModule(DiscordShardedClient client, IServiceProvider services) : base(client, services)
    {
        Initialize().GetAwaiter();
    }

    private new async Task Initialize()
    {
        await InteractionService.AddModuleAsync(typeof(Settings), Service);
        CommandHandler = new CommandHandler(InteractionService, Client, Service);
    }
    
}