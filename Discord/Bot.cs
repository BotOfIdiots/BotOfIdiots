using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Discord.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Discord
{
    internal class Bot
    {
        public static ServiceProvider Services;
        
        public static ulong ControleGuild;

        public Bot()
        {
            using var services = ConfigureServices();

            var config = services.GetRequiredService<XmlDocument>();
            var client = services.GetRequiredService<DiscordShardedClient>();
            var moduleManager = services.GetRequiredService<ModuleManager>();

            client.LoginAsync(TokenType.Bot, config.SelectSingleNode("config/BotToken").InnerText).GetAwaiter();
            moduleManager.Initialize().GetAwaiter();

            client.Log += ClientLog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StartBotAsync()
        {
            var client = Services.GetRequiredService<IDiscordClient>();

            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        private XmlDocument LoadSettings()
        {
            XmlDocument settings = new XmlDocument();
            
            switch ((int)Environment.OSVersion.Platform)
            {
                case 4:
                    settings.Load("../config/config.xml");
                    break;
                case 2:
                    settings.Load(Base.WorkingDirectory + "/config.xml");
                    break;
            }
            
            ControleGuild = Convert.ToUInt64(settings.DocumentElement["ControleGuild"].Value);

            return settings;
        }

        private ServiceProvider ConfigureServices()
        {
            XmlDocument settings = LoadSettings();

            var DiscordSettings = settings.DocumentElement["DiscordSocketConfig"].ChildNodes;

            var discordSocketConfig =
                BuildDiscordSocketConfig(DiscordSettings);
            int[] shardId = { Convert.ToInt32(DiscordSettings[2].InnerText) };

            Services = new ServiceCollection()
                .AddSingleton<XmlDocument>(settings)
                .AddSingleton<IDiscordClient>(new DiscordShardedClient(shardId, discordSocketConfig))
                .AddSingleton<ModuleManager>()
                .AddSingleton<DatabaseService>()
                .BuildServiceProvider();

            return Services;
        }

        private DiscordSocketConfig BuildDiscordSocketConfig(XmlNodeList settings)
        {
            DiscordSocketConfig discordSocketConfig = new DiscordSocketConfig();
            discordSocketConfig.AlwaysDownloadUsers = true;
            discordSocketConfig.GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.GuildPresences;

            foreach (XmlNode node in settings)
            {
                switch (node.Name)
                {
                    case "MessageCacheSize":
                        discordSocketConfig.MessageCacheSize = Convert.ToInt32(node.InnerText);
                        break;
                    case "TotalShards":
                        discordSocketConfig.TotalShards = Convert.ToInt32((node.InnerText));
                        break;
                }
            }

            return discordSocketConfig;
        }

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task ClientLog(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        #endregion
    }
}