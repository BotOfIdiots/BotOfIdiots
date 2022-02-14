using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Modules.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    internal class DiscordBot
    {
        #region Fields
        
        private static readonly string _version = "0.0.7";
        public static ServiceProvider Services;
        public static string WorkingDirectory = Directory.GetCurrentDirectory();
        public static ulong ControleGuild;

        #endregion

        #region Main Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            new DiscordBot().RunBotAsync(args).GetAwaiter().GetResult();
        } 

        #endregion

        #region Startup Logic

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RunBotAsync(string[] args)
        {
            using var services = ConfigureServices();

            var config = services.GetRequiredService<XmlDocument>();
            var client = services.GetRequiredService<DiscordShardedClient>();
            var commands = services.GetRequiredService<InteractionService>();
            var handler = services.GetRequiredService<CommandHandler>();

#pragma warning disable CS4014
            handler.Initialize();
#pragma warning restore CS4014

            await client.LoginAsync(TokenType.Bot, config.SelectSingleNode("config/BotToken").InnerText);

            client.Log += ClientLog;
            commands.Log += (msg) =>
            {
                Console.WriteLine(msg.ToString());
                return Task.CompletedTask;
            };

            await client.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        private ServiceProvider ConfigureServices()
        {
            XmlDocument settings = new XmlDocument();
            settings.Load(WorkingDirectory + "/config.xml");

            var DiscordSettings = settings.DocumentElement["DiscordSocketConfig"].ChildNodes;
            
            var discordSocketConfig =
                BuildDiscordSocketConfig(DiscordSettings);
            int[] shardId = { Convert.ToInt32(Convert.ToInt32(DiscordSettings[2].InnerText)) };

            Services = new ServiceCollection()
                .AddSingleton<XmlDocument>(settings)
                .AddSingleton<DiscordShardedClient>(new DiscordShardedClient(shardId, discordSocketConfig))
                .AddSingleton<InteractionService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<DatabaseService>()
                .BuildServiceProvider();

            return Services;
        }
        
        private DiscordSocketConfig BuildDiscordSocketConfig(XmlNodeList settings)
        {
            DiscordSocketConfig discordSocketConfig = new DiscordSocketConfig();
            discordSocketConfig.AlwaysDownloadUsers = true;
            discordSocketConfig.GatewayIntents = GatewayIntents.AllUnprivileged;

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

        #endregion

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Version()
        {
            return _version;
        }

        #endregion
    }
}