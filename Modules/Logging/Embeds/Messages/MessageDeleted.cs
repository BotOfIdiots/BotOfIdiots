using Discord;
using EmbedAuthor = DiscordBot.Modules.Base.Embeds.EmbedAuthor;

namespace DiscordBot.Modules.Logging.Embeds.Messages
{
    public class MessageDeleted : EmbedBuilder
    {
        private readonly IMessage _message;
        public MessageDeleted(Cacheable<IMessage, ulong> cachedMessage)
        {
            _message = cachedMessage.Value;
            IChannel channel = _message.Channel;
            
            Title = "Message Deleted";
            
            MessageHasContent();
            AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")");

            WithAuthor(new EmbedAuthor(_message.Author));
            WithColor(Discord.Color.Red);
            WithFooter("MessageID: " + cachedMessage.Id);
            WithCurrentTimestamp();
        }

        private void MessageHasContent()
        {
            if (_message.Content != null)
            {
                WithDescription(_message.Content);
                AddField("Sent At", _message.Timestamp.ToLocalTime().ToString("HH:mm:ss dd-MM-yyyy"));
            }
            else
            {
                WithDescription("Could not retrieve message from cache");
            }
        }
    }
}