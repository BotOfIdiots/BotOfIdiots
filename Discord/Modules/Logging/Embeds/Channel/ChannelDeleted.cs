using Discord;
using Discord.WebSocket;

namespace DiscordBot.Discord.Modules.Logging.Embeds.Channel
{
    public class ChannelDeletedEmbedBuilder : EmbedBuilder
    {
        #region Constructors
        public ChannelDeletedEmbedBuilder(SocketGuildChannel channel)
        {
            WithTitle("Channel Deleted");
            WithDescription("A Channel has been removed from the Guild");
            WithColor(global::Discord.Color.Red);
            AddField("Channel name: ", channel.Name);
            WithCurrentTimestamp();
            WithFooter("ChannelId: " + channel.Id);
        }
        #endregion
    }
    
}