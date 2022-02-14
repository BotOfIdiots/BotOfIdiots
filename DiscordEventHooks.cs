using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Modules;
using DiscordBot.Modules.Commands;

namespace DiscordBot
{
    public class DiscordEventHooks
    {
        public CommandHandler CommandHandler;

        public DiscordEventHooks(DiscordShardedClient client, DatabaseService databaseService)
        {
            ClientEvents(client);
            MemberEvents(client);
            MessageEvents(client);
            BanEvents(client);
            ChannelEvents(client);
            CommandEvents(client);
        }
        
        
        public void ClientEvents(DiscordShardedClient client)
        {
            client.JoinedGuild += EventHandlers.ClientJoinGuildHandler;
            client.ShardReady += EventHandlers.Ready;
        }
        
        public void MemberEvents(BaseSocketClient client)
        {
            //Member joined and leave logging hooks
            client.UserJoined += EventHandlers.MemberJoinGuildHandler;
            client.UserLeft += EventHandlers.MemberLeaveGuildHandler;
            
            //Member update logging hook
            client.GuildMemberUpdated += EventHandlers.MemberUpdatedHandler;
            
            //Voicestate logging hook
            client.UserVoiceStateUpdated += EventHandlers.MemberVoiceStateHandler;
        }
        
        public void MessageEvents(BaseSocketClient client)
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

        public void BanEvents(BaseSocketClient client)
        {
            client.UserUnbanned += EventHandlers.MemberUnbannedHandler;
            client.UserBanned += EventHandlers.MemberBannedHandler;
        }
        
        public void ChannelEvents(DiscordShardedClient client)
        {
            // client.ChannelUpdated += EventHandlers.ChannelUpdateHandler;
            client.ChannelCreated += EventHandlers.ChannelCreatedHandler;
            client.ChannelDestroyed += EventHandlers.ChannelDeleteHandler;
        }

        public void CommandEvents(DiscordShardedClient client)
        {
            // client.SlashCommandExecuted += CommandHandler.HandleSlashCommandAsync;
        }
    }
}