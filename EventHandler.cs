using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot
{
    public static class EventHandler
    {
        public static void HookMessageDeleted(BaseSocketClient client) 
            => client.MessageDeleted += Logger.MessageDeleteHandler;

        public static void HookMessageUpdated(BaseSocketClient client)
            => client.MessageUpdated += Logger.MessageUpdateHandler;
    }
}