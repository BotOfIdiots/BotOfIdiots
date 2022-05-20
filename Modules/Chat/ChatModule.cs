using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Modules.Base;
using DiscordBot.Modules.Chat.Commands;

namespace DiscordBot.Modules.Chat;

public class ChatModule : BaseModule
{
    public ChatModule(DiscordShardedClient client, IServiceProvider service) : base(client, service)
    {
        Initialize().GetAwaiter();
    }

    private new async Task Initialize()
    {
        await InteractionService.AddModuleAsync(typeof(Settings), Service);
        CommandHandler = new CommandHandler(InteractionService, Client, Service);
    }
}