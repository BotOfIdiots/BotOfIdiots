using Discord;
using Discord.WebSocket;

namespace DiscordBot.Objects.Embeds.Channel
{
    public class ChannelDeletedEmbedBuilder : EmbedBuilder
    {
        #region Constructors
        public ChannelDeletedEmbedBuilder(SocketGuildChannel channel)
        {
            WithTitle("Channel Deleted");
            WithDescription("A Channel has been removed from the Guild");
            WithColor(Discord.Color.Red);
            AddField("Channel name: ", channel.Name);
            WithCurrentTimestamp();
            WithFooter("ChannelId: " + channel.Id);
        }
        #endregion
    }
    
}