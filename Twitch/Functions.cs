using DiscordBot.DiscordApi.Modules;

namespace DiscordBot.Twitch
{
    public static class Functions
    {
        public static void SendDebugMessage(string message)
        {
            EventHandlers.LogDebugMessage(message);
        }
    }
}