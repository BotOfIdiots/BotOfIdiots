using Discord;
using Discord.WebSocket;

namespace DiscordBot.Models.Embeds.Messages
{
    public class MessageUpdated : EmbedBuilder
    {
        private readonly IMessage _oldMessage;
        private readonly IMessage _newMessage;
        
        public MessageUpdated(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message)
        {
            _oldMessage = cachedMessage.Value;
            _newMessage = message;
            IChannel channel = message.Channel;
            
            Title = "Message Updated";

            BuildMessageFields();
            AddField("Channel", "<#" + channel.Id + "> (" + channel.Name + "/" + channel.Id + ")");

            WithAuthor(new EmbedAuthor(message.Author));
            WithColor(Discord.Color.Orange);
            WithFooter("MessageID: " + cachedMessage.Id);
            WithCurrentTimestamp();
        }

        private void BuildMessageFields()
        {
            if (_oldMessage != null && _newMessage != null)
            {
                AddField("Old Content", _oldMessage.Content);
                AddField("New Content", _newMessage.Content);
                AddField("Sent At", _newMessage.Timestamp.ToLocalTime()
                    .ToString("HH:mm:ss dd-MM-yyyy"));
            }
            else if (_oldMessage == null)
            {
                AddField("Old Content", "Could not retrieve message from cache");
                AddField("New Content", _newMessage.Content);
                AddField("Sent At", _newMessage.Timestamp.ToLocalTime()
                    .ToString("HH:mm:ss dd-MM-yyyy"));
            }
            else if (_newMessage == null)
            {
                AddField("Old Content", _oldMessage.Content);
                AddField("New Content", "Could not retrieve message from cache");
                AddField("Sent At", _newMessage.Timestamp.ToLocalTime()
                    .ToString("HH:mm:ss dd-MM-yyyy"));
            }
            else
            {
                WithDescription("Could not retrieve message from cache");
            }
            
        }
    }
}