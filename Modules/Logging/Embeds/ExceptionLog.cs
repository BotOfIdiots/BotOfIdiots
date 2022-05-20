using Discord;

namespace DiscordBot.Modules.Logging.Embeds
{
    public class ExceptionLog : EmbedBuilder
    {
        public ExceptionLog(System.Exception exception)
        {
            Title = exception.Message;
            Description = exception.StackTrace;
            AddField("Source", exception.Source);
            WithColor(Discord.Color.Red);
            WithFooter(DiscordBot.Version());
            WithCurrentTimestamp();
        }
    }
}