using Discord.WebSocket;

namespace DiscordBot.Discord.Modules.Event
{
    public class DiscordEventHooks
    {

        public DiscordEventHooks(DiscordShardedClient client)
        {
            ClientEvents(client);
            MemberEvents(client);
            MessageEvents(client);
            BanEvents(client);
            ChannelEvents(client);
        }


        private void ClientEvents(DiscordShardedClient client)
        {
            client.JoinedGuild += EventHandlers.ClientJoinGuildHandler;
        }

        private void MemberEvents(BaseSocketClient client)
        {
            //Member joined and leave logging hooks
            client.UserJoined += EventHandlers.MemberJoinGuildHandler;
            client.UserLeft += EventHandlers.MemberLeaveGuildHandler;
            
            //Member update logging hook
            client.GuildMemberUpdated += EventHandlers.MemberUpdatedHandler;
            
            //Voicestate logging hook
            client.UserVoiceStateUpdated += EventHandlers.MemberVoiceStateHandler;
        }

        private void MessageEvents(BaseSocketClient client)
        {
            //Message logging hooks
            client.MessageDeleted += EventHandlers.MessageDeleteHandler;
            client.MessagesBulkDeleted += EventHandlers.MessageBulkDeleteHandler;
            client.MessageUpdated += EventHandlers.MessageUpdateHandler;
            
            //Reaction role hooks
            client.ReactionAdded += EventHandlers.ReactionAddedHandler;
            client.ReactionRemoved += EventHandlers.ReactionRemovedHandler;
            
            //Level system hooks
            // client.ReactionAdded += Levels.AddReactionXp;
            // client.ReactionRemoved += Levels.RemoveReactionXp;
        }

        private void BanEvents(BaseSocketClient client)
        {
            client.UserUnbanned += EventHandlers.MemberUnbannedHandler;
            client.UserBanned += EventHandlers.MemberBannedHandler;
        }

        private void ChannelEvents(DiscordShardedClient client)
        {
            // client.ChannelUpdated += EventHandlers.ChannelUpdateHandler;
            client.ChannelCreated += EventHandlers.ChannelCreatedHandler;
            client.ChannelDestroyed += EventHandlers.ChannelDeleteHandler;
        }
    }
}