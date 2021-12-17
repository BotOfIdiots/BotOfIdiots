using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    internal class DiscordBot
    {
        #region Fields

        private static readonly string _version = "0.0.7";
        private static IServiceProvider _services;
        public static string WorkingDirectory;
        public static DiscordShardedClient ShardedClient;
        public static CommandService Commands;
        public static IConfiguration Config;
        public static DiscordSocketConfig DiscordConfig;
        private XmlDocument settings = new XmlDocument();
        public static DbConnection DbConnection;

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
            // WorkingDirectory = args.GetValue(0).ToString();
            WorkingDirectory = Directory.GetCurrentDirectory();
            _loadSettings();

            DbConnection = new DbConnection(settings.DocumentElement["SQLSettings"].ChildNodes);
            
            DiscordConfig = BuildBotConfig(settings.DocumentElement["DiscordConfig"].ChildNodes);
            int[] shardId = { Convert.ToInt32(settings.DocumentElement["ShardId"].InnerText) };

            ShardedClient = new DiscordShardedClient(shardId, DiscordConfig);
            Commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(ShardedClient)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            ShardedClient.Log += ClientLog;
            LoadDiscordEventHandlers();

            await RegisterCommandsAsync();
            await ShardedClient.LoginAsync(TokenType.Bot, settings.SelectSingleNode("config/BotToken").InnerText);
            await ShardedClient.StartAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Register all the Discord Event Hooks
        /// </summary>
        private void LoadDiscordEventHandlers()
        {
            DiscordEventHooks.HookClientEvents(ShardedClient);
            DiscordEventHooks.HookMessageEvents(ShardedClient);
            DiscordEventHooks.HookMemberEvents(ShardedClient);
            DiscordEventHooks.HookChannelEvents(ShardedClient);
            DiscordEventHooks.HookBanEvents(ShardedClient);
        }

        /// <summary>
        /// Create the config object based on the config.json file
        /// </summary>
        private void _loadSettings()
        {
            settings.Load(WorkingDirectory + "/config.xml");
            var builder = new ConfigurationBuilder()
                .SetBasePath(WorkingDirectory)
                .AddJsonFile(path: "config.json");
            Config = builder.Build();
            Config = Config.GetSection("DiscordBot");
        }

        private DiscordSocketConfig BuildBotConfig(XmlNodeList settings)
        {
            bool exclusiveBulkDelete = false;
            int messageCacheSize = 0;
            int totalShards = 0;

            foreach (XmlNode node in settings)
            {
                switch (node.Name)
                {
                    case "ExclusiveBulkDelete":
                        exclusiveBulkDelete = Convert.ToBoolean(node.InnerText);
                        break;
                    case "MessageCacheSize":
                        messageCacheSize = Convert.ToInt32(node.InnerText);
                        break;
                    case "TotalShards":
                        totalShards = Convert.ToInt32((node.InnerText));
                        break;
                }
            }

            return new DiscordSocketConfig
            {
                ExclusiveBulkDelete = exclusiveBulkDelete,
                MessageCacheSize = messageCacheSize,
                TotalShards = totalShards
            };
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
            ShardedClient.MessageReceived += HandleCommandAsync;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Version()
        {
            return _version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new ShardedCommandContext(ShardedClient, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("$", ref argPos))
            {
                var result = await Commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Embed exceptionEmbed = new EmbedBuilder()
                        .WithColor(Color.Red)
                        .WithDescription(result.ErrorReason)
                        .Build();

                    await context.Channel.SendMessageAsync(embed: exceptionEmbed);
                }

                if (result.IsSuccess)
                {
                    await EventHandlers.LogExecutedCommand(context, message);
                }
            }
        }
        #endregion
    }
}