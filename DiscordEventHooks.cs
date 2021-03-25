using System.Diagnostics;
using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot
{
    public static class DiscordEventHooks
    {
        public static void HookMessageDeleted(BaseSocketClient client) 
            => client.MessageDeleted += EventHandlers.MessageDeleteHandler;

        public static void HookMessageBulkDeleted(BaseSocketClient client)
            => client.MessagesBulkDeleted += EventHandlers.MessageBulkDeleteHandler;
        
        public static void HookMessageUpdated(BaseSocketClient client)
            => client.MessageUpdated += EventHandlers.MessageUpdateHandler;

        public static void HookMemberJoinGuild(BaseSocketClient client)
            => client.UserJoined += EventHandlers.MemberJoinGuildHandler;

        public static void HookMemberLeaveGuild(BaseSocketClient client)
            => client.UserLeft += EventHandlers.MemberLeaveGuildHandler;

        public static void HookMemberVoiceState(BaseSocketClient client)
            => client.UserVoiceStateUpdated += EventHandlers.MemberVoiceStateHandler;

        public static void HookMemberUpdated(BaseSocketClient client)
            => client.GuildMemberUpdated += EventHandlers.MemberUpdatedHandler;

        public static void HookMemberBanned(BaseSocketClient client)
            => client.UserBanned += EventHandlers.MemberBannedHandler;

        public static void HookMemberUnbanned(DiscordSocketClient client)
            => client.UserUnbanned += EventHandlers.MemberUnbannedHandler;
    }
}