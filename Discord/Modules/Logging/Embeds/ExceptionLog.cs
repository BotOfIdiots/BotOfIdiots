using Discord;

namespace DiscordBot.Discord.Modules.Logging.Embeds
{
    public class ExceptionLog : EmbedBuilder
    {
        public ExceptionLog(System.Exception exception)
        {
            Title = exception.Message;
            Description = exception.StackTrace;
            AddField("Source", exception.Source);
            WithColor(global::Discord.Color.Red);
            WithFooter("Add footer");
            WithCurrentTimestamp();
        }
    }
}