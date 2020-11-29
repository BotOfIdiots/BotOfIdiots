using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot
{
    public static class DiscordEventHandler
    {
        public static void HookMessageDeleted(BaseSocketClient client) 
            => client.MessageDeleted += Logger.MessageDeleteHandler;

        public static void HooMessageBulkDelted(BaseSocketClient client)
            => client.MessagesBulkDeleted += Logger.MessageBulkDeleteHandler;
        
        public static void HookMessageUpdated(BaseSocketClient client)
            => client.MessageUpdated += Logger.MessageUpdateHandler;

        public static void HookMemberJoin(BaseSocketClient client)
            => client.UserJoined += Logger.MemberJoinHandler;

        public static void HookMemberLeave(BaseSocketClient client)
            => client.UserLeft += Logger.MemberLeaveHandler;
    }
}