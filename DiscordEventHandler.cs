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

        public static void HookMemberJoinGuild(BaseSocketClient client)
            => client.UserJoined += Logger.MemberJoinGuildHandler;

        public static void HookMemberLeaveGuild(BaseSocketClient client)
            => client.UserLeft += Logger.MemberLeaveGuildHandler;

        public static void HookMemberVoiceState(BaseSocketClient client)
            => client.UserVoiceStateUpdated += Logger.MemberVoiceStateHandler;
    }
}