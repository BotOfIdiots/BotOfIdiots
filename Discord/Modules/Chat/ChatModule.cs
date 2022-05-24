using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Discord.Modules.Base;
using DiscordBot.Discord.Modules.Chat.Commands;

namespace DiscordBot.Discord.Modules.Chat;

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