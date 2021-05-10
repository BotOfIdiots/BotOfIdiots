using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models.Embeds
{
    public class ChannelDeletedEmbedBuilder : EmbedBuilder
    {
        public ChannelDeletedEmbedBuilder(SocketGuildChannel channel)
        {
            WithTitle("Channel Deleted");
            WithDescription("A Channel has been removed from the Guild");
            WithColor(Discord.Color.Red);
            AddField("Channel name: ", channel.Name);
            WithCurrentTimestamp();
            WithFooter("ChannelId: " + channel.Id);
        }
    }
    
}