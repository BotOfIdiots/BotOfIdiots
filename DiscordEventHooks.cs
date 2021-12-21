using Discord.WebSocket;
using DiscordBot.Modules;

namespace DiscordBot
{
    public static class DiscordEventHooks
    {
        public static void HookClientEvents(DiscordShardedClient client)
        {
            client.JoinedGuild += EventHandlers.ClientJoinGuildHandler;
        }
        
        public static void HookMemberEvents(BaseSocketClient client)
        {
            //Member joined and leave logging hooks
            client.UserJoined += EventHandlers.MemberJoinGuildHandler;
            client.UserLeft += EventHandlers.MemberLeaveGuildHandler;
            
            //Member update logging hook
            client.GuildMemberUpdated += EventHandlers.MemberUpdatedHandler;
            
            //Voicestate logging hook
            client.UserVoiceStateUpdated += EventHandlers.MemberVoiceStateHandler;
        }
        
        public static void HookMessageEvents(BaseSocketClient client)
        {
            //Message logging hooks
            client.MessageDeleted += EventHandlers.MessageDeleteHandler;
            client.MessagesBulkDeleted += EventHandlers.MessageBulkDeleteHandler;
            client.MessageUpdated += EventHandlers.MessageUpdateHandler;
            
            //Reaction role hooks
            client.ReactionAdded += EventHandlers.ReactionAddedHandler;
            client.ReactionRemoved += EventHandlers.ReactionRemovedHandler;
            
            //Level system hooks
            client.ReactionAdded += Levels.AddReactionXp;
            client.ReactionRemoved += Levels.RemoveReactionXp;
        }

        public static void HookBanEvents(BaseSocketClient client)
        {
            client.UserUnbanned += EventHandlers.MemberUnbannedHandler;
            client.UserBanned += EventHandlers.MemberBannedHandler;
        }
        
        public static void HookChannelEvents(DiscordShardedClient client)
        {
            // client.ChannelUpdated += EventHandlers.ChannelUpdateHandler;
            client.ChannelCreated += EventHandlers.ChannelCreatedHandler;
            client.ChannelDestroyed += EventHandlers.ChannelDeleteHandler;
        }

        public static void CommandEvents(DiscordShardedClient client)
        {
            client.SlashCommandExecuted += CommandHandler.HandleSlashCommandAsync;
            client.MessageReceived += CommandHandler.HandleCommandAsync;
        }
    }
}