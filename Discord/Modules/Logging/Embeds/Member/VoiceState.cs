using System;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Discord.Modules.Logging.Embeds.Member
{
    public class VoiceStateEmbedBuilder : EmbedBuilder
    {
      
        #region Constructors
        public VoiceStateEmbedBuilder(int state, SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            AddField("User", user.Mention);
            
            switch (state)
            {
                case 0:
                    _leftChannelEmbed(before);
                    break;
                case 1:
                    _joinedChannelEmbed(after);
                    break;
                case 2:
                    _switchedChannelEmbed(before, after);
                    break;
                default:
                    throw new Exception("Unknown voice state");
            }
            WithCurrentTimestamp();
            WithFooter("UserID: " + user.Id);
        }
        #endregion

        #region Methods
        private void _joinedChannelEmbed(SocketVoiceState state)
        {
            WithColor(global::Discord.Color.Green);
            WithTitle("User joined a channel");
            AddField("Channel", state.VoiceChannel.Name + " (" + state.VoiceChannel.Id + ")");
        }

        private void _leftChannelEmbed(SocketVoiceState state)
        {
            WithColor(global::Discord.Color.Red);
            WithTitle("User left a channel");
            AddField("Channel", state.VoiceChannel.Name + " (" + state.VoiceChannel.Id + ")");
        }

        private void _switchedChannelEmbed(SocketVoiceState before, SocketVoiceState after)
        {
            WithColor(global::Discord.Color.Blue);
            WithTitle("User switched channels");
            AddField("Origin", before.VoiceChannel.Name + " (" + before.VoiceChannel.Id + ")");
            AddField("Destination", after.VoiceChannel.Name + " (" + after.VoiceChannel.Id + ")");
        }
        #endregion
    }
}