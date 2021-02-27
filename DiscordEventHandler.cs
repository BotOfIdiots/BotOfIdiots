using System.Diagnostics;
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

        public static void HookMemberUpdated(BaseSocketClient client)
            => client.GuildMemberUpdated += Logger.MemberUpdatedHandler;

        public static void HookMemberBanned(BaseSocketClient client)
            => client.UserBanned += Logger.MemberBannedHandler;

        public static void HookMemberUnbanned(DiscordSocketClient client)
            => client.UserUnbanned += Logger.MemberUnbannedHandler;
    }
}