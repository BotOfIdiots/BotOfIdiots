using Discord;
using Discord.Commands;

namespace DiscordBot.Objects.Embeds
{
    public class ExecutedCommand : EmbedBuilder
    {
        #region Constructors
        public ExecutedCommand(SocketCommandContext context, IMessage message)
        {
            IUser author = message.Author;
            
            WithAuthor(author.Username + "#" + author.Discriminator, author.GetAvatarUrl());
            WithTitle("Command successfully executed");
            WithColor(Discord.Color.DarkBlue);
            WithDescription(message.Content);
            AddField("Origin Channel", "<#" + message.Channel.Id + "> (" + message.Channel.Id + ")");
            WithCurrentTimestamp();
            WithFooter("UserID: " + author.Id);
        }
        #endregion
    }
}