using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace DiscordBot.Twitch.Modules
{
    public class LiveMonitor
    {
        
        private LiveStreamMonitorService _monitor;
        private TwitchAPI _api;
        private bool _onlineCheckEnabled;
        private bool _offlineCheckEnabled;
        private bool _updateCheckEnabled;

        public LiveMonitor()
        {
            EnableModules();
            ConfigMonitor();
        }

        private void EnableModules()
        {
            var modules = Base.TwitchIntegration.Config.GetSection("Modules");
            _onlineCheckEnabled = Convert.ToBoolean(modules["StreamOnline"]);
            _offlineCheckEnabled = Convert.ToBoolean(modules["StreamOffline"]);
            _updateCheckEnabled = Convert.ToBoolean(modules["StreamUpdated"]);
        }
        
        private void ConfigMonitor()
        {
            _api = new TwitchAPI();

            _api.Settings.ClientId = Base.TwitchIntegration.AccessToken.ClientId;
            _api.Settings.AccessToken = Base.TwitchIntegration.AccessToken.Token;

            //TODO: Make interval changeable from config
            _monitor = new LiveStreamMonitorService(_api, 10);

            
            List<string> channelList = new List<string> {"SideShadow63"};
            _monitor.SetChannelsByName(channelList);

            _monitor.OnServiceStarted += ServiceStarted;
            _monitor.OnServiceTick += ServiceCheck;

            if (_onlineCheckEnabled)
            {
                _monitor.OnStreamOnline += StreamOnline;
                Console.WriteLine("Stream Online enabled");
            }

            if (_offlineCheckEnabled)
            {
                _monitor.OnStreamOffline += StreamOffline;
                Console.WriteLine("Stream Offline enabled");
            }

            if (_updateCheckEnabled)
            {
                _monitor.OnStreamUpdate += StreamUpdated;
                Console.WriteLine("Stream Update enabled");
            }
        }

        public async Task StartMonitorAsync()
        {
            _monitor.Start();
            var channelList = _monitor.ChannelsToMonitor.ToList();
            foreach (string channel in channelList)
            {
                Console.WriteLine(channel);
            }
            
            await Task.Delay(-1);
        } 
        
        public async Task StopMonitorAsync()
        {
            _monitor.Stop();
            await Task.Delay(-1);
        }

        private void ServiceStarted(object sender, OnServiceStartedArgs serviceArgs)
        {
            Functions.SendDebugMessage("Live Stream Monitor Online");
        }

        private void ServiceCheck(object sender, OnServiceTickArgs serviceArgs)
        {
            if (Base.TwitchIntegration.Debug)
            {
                Functions.SendDebugMessage("Hello There");
            }
        }
        
        private void StreamOnline(object sender, OnStreamOnlineArgs onlineArgs)
        {
            // TODO: Implement stream online method
           Console.WriteLine("Live Stream detected"); 
           Functions.SendDebugMessage("Live Stream Online");
        }

        private void StreamOffline(object sender, OnStreamOfflineArgs offlineArgs)
        {
            //TODO: Implement stream offline method
            Console.WriteLine("Live Stream detected");
            Functions.SendDebugMessage("Live Stream Offline");
        }

        private void StreamUpdated(object sender, OnStreamUpdateArgs updateArgs)
        {
            //TODO: Implement stream updated method
        }
    }
}