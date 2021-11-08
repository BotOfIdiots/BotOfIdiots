using System;
using DiscordBot.Twitch.Models;
using Microsoft.Extensions.Configuration;
using TwitchLib.Api;

namespace DiscordBot.Twitch
{
    public class TwitchIntegration
    { 
        public IConfiguration Config;
        public TwitchAPI Api = new TwitchAPI();
        public readonly TwitchApiToken AccessToken;
        public bool Debug = false;
        
        public TwitchIntegration(IConfiguration config)
        {
            Config = config;
            AccessToken = new TwitchOauth(Config["ClientId"], Config["Secret"]).Authenticate();

            Api.Settings.ClientId = Config["ClientId"];
            Api.Settings.Secret = Config["Secret"];
        }

        public async void StartAsync()
        {
            
        }
    }
}