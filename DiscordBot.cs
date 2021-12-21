using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    internal class DiscordBot
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        private static readonly string _version = "0.0.7";
        
        /// <summary>
        /// 
        /// </summary>
        private static int[] _shardId;
        
        /// <summary>
        /// 
        /// </summary>
        private static DiscordSocketConfig _discordSocketConfig;
        
        /// <summary>
        /// 
        /// </summary>
        private static IConfiguration _config;
        
        /// <summary>
        /// 
        /// </summary>
        private XmlDocument settings = new XmlDocument();
        
        /// <summary>
        /// 
        /// </summary>
        public static IServiceProvider Services;
        
        /// <summary>
        /// 
        /// </summary>
        private static DiscordShardedClient _client;

        /// <summary>
        /// 
        /// </summary>
        private static CommandService _commandService;

        /// <summary>
        /// 
        /// </summary>
        private static DatabaseService _databaseService;
        
        /// <summary>
        /// 
        /// </summary>
        public static string WorkingDirectory;
        
        /// <summary>
        /// 
        /// </summary>
        public static ulong ControleGuild;
        
        #endregion

        #region Main Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => new DiscordBot().RunBotAsync(args).GetAwaiter().GetResult();

        #endregion

        #region Startup Logic

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RunBotAsync(string[] args)
        {
            WorkingDirectory = Directory.GetCurrentDirectory();
            _loadSettings();

            BuildDiscordSocketConfig(settings.DocumentElement["DiscordSocketConfig"].ChildNodes);
            ControleGuild = Convert.ToUInt64(settings.DocumentElement["ControleGuild"].InnerText);

            _client = new DiscordShardedClient(_shardId, _discordSocketConfig);
            _commandService = new CommandService();
            _databaseService = new DatabaseService(settings.DocumentElement["SQLSettings"].ChildNodes);

            Services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(_databaseService)
                .AddSingleton(new DiscordEventHooks(_client, _databaseService))
                .BuildServiceProvider();

            _client.Log += ClientLog;
            
            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, settings.SelectSingleNode("config/BotToken").InnerText);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Register all the Discord Event Hooks
        /// </summary>

        /// <summary>
        /// Create the config object based on the config.json file
        /// </summary>
        private void _loadSettings()
        {
            settings.Load(WorkingDirectory + "/config.xml");
            var builder = new ConfigurationBuilder()
                .SetBasePath(WorkingDirectory)
                .AddJsonFile(path: "config.json");
            _config = builder.Build();
            _config = _config.GetSection("DiscordBot");
        }

        private void BuildDiscordSocketConfig(XmlNodeList settings)
        {
            _discordSocketConfig = new DiscordSocketConfig();

            foreach (XmlNode node in settings)
            {
                switch (node.Name)
                {
                    case "MessageCacheSize":
                        _discordSocketConfig.MessageCacheSize = Convert.ToInt32(node.InnerText);
                        break;
                    case "TotalShards":
                        _discordSocketConfig.TotalShards = Convert.ToInt32((node.InnerText));
                        break;
                    case "ShardId":
                        _shardId = new int[] { Convert.ToInt32(node.InnerText) };
                        break;
                }
            }
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
        public async Task RegisterCommandsAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
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