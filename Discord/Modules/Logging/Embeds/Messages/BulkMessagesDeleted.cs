using System.Collections.Generic;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using EmbedAuthor = DiscordBot.Discord.Modules.Base.Embeds.EmbedAuthor;

namespace DiscordBot.Discord.Modules.Logging.Embeds.Messages
{
    public class BulkMessagesDeleted : EmbedBuilder
    {
        public BulkMessagesDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> cachedData, ISocketMessageChannel channel)
        {
            Title = "Bulk Messages Delete";
            
            AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")");
            AddField("Amount", cachedData.Count);

            WithAuthor(new EmbedAuthor(Bot.Services.GetRequiredService<DiscordShardedClient>().CurrentUser));
            WithColor(global::Discord.Color.Red);
            WithFooter("Message Count: " + cachedData.Count);
            WithCurrentTimestamp();
        }
    }
}