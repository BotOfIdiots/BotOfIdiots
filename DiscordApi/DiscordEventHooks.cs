using System.Diagnostics;
using Discord.WebSocket;
using DiscordBot.DiscordApi.Modules;
using DiscordBot.Modules;

namespace DiscordBot
{
    public static class DiscordEventHooks
    {
        public static void HookClientEvents(BaseSocketClient client)
        {
            client.JoinedGuild += EventHandlers.ClientJoinGuildHandler;
        }
        
        public static void HookMemberEvents(BaseSocketClient client)
        {
            client.UserJoined += EventHandlers.MemberJoinGuildHandler;
            client.UserLeft += EventHandlers.MemberLeaveGuildHandler;
            client.GuildMemberUpdated += EventHandlers.MemberUpdatedHandler;
            client.UserVoiceStateUpdated += EventHandlers.MemberVoiceStateHandler;
        }
        
        public static void HookMessageEvents(BaseSocketClient client)
        {
            client.MessageDeleted += EventHandlers.MessageDeleteHandler;
            client.MessagesBulkDeleted += EventHandlers.MessageBulkDeleteHandler;
            client.MessageUpdated += EventHandlers.MessageUpdateHandler;
            client.ReactionAdded += EventHandlers.ReactionAddedHandler;
            client.ReactionRemoved += EventHandlers.ReactionRemovedHandler;
        }

        public static void HookBanEvents(BaseSocketClient client)
        {
            client.UserUnbanned += EventHandlers.MemberUnbannedHandler;
            client.UserBanned += EventHandlers.MemberBannedHandler;
        }
        
        public static void HookChannelEvents(DiscordSocketClient client)
        {
            // client.ChannelUpdated += EventHandlers.ChannelUpdateHandler;
            client.ChannelCreated += EventHandlers.ChannelCreatedHandler;
            client.ChannelDestroyed += EventHandlers.ChannelDeleteHandler;
        }
    }
}