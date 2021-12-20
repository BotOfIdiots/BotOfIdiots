using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Objects.Embeds.Messages
{
    public class BulkMessagesDeleted : EmbedBuilder
    {
        public BulkMessagesDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedData, ISocketMessageChannel channel)
        {
            Title = "Bulk Messages Delete";
            
            AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")");
            AddField("Amount", cachedData.Count);

            WithAuthor(new EmbedAuthor(DiscordBot.ShardedClient.CurrentUser));
            WithColor(Discord.Color.Red);
            WithFooter("Message Count: " + cachedData.Count);
            WithCurrentTimestamp();
        }
    }
}