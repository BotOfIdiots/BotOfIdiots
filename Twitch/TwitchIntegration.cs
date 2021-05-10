using DiscordBot.Models;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Twitch
{
    public class TwitchIntegration
    { 
        public IConfiguration Config;
        public bool Debug = false;
        public TwitchApiToken AccessToken;
        
        public TwitchIntegration(IConfiguration config)
        {
            Config = config;
            AccessToken = new TwitchOauth(Config["ClientId"], Config["Secret"]).Authenticate();
        }

        public async void StartAsync()
        {
            
        }
    }
}