using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using EmbedAuthor = DiscordBot.Modules.Base.Embeds.EmbedAuthor;

namespace DiscordBot.Modules.Logging.Embeds.Messages
{
    public class BulkMessagesDeleted : EmbedBuilder
    {
        public BulkMessagesDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedData, ISocketMessageChannel channel)
        {
            Title = "Bulk Messages Delete";
            
            AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")");
            AddField("Amount", cachedData.Count);

            WithAuthor(new EmbedAuthor(DiscordBot.Services.GetRequiredService<DiscordShardedClient>().CurrentUser));
            WithColor(Discord.Color.Red);
            WithFooter("Message Count: " + cachedData.Count);
            WithCurrentTimestamp();
        }
    }
}