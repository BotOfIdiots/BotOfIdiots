using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Discord.Modules.Base;
using DiscordBot.Discord.Modules.Logging.Commands;

namespace DiscordBot.Discord.Modules.Logging;

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