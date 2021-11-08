using System;
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
        #region Main Method
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => new DiscordBot().RunBotAsync().GetAwaiter().GetResult();
        #endregion
        
        #region Fields
        private static readonly string _version = "0.0.7";
        private static IServiceProvider _services;
        public static string WorkingDirectory;
        public static DiscordSocketClient Client;
        public static CommandService Commands;
        public static IConfiguration Config;
        private XmlDocument settings = new XmlDocument();
        public static DbConnection DbConnection;
        #endregion
        
        #region Startup Logic
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RunBotAsync()
        {
            _setWorkingDirectory(_detectOS());
            _loadSettings();

            DbConnection = new DbConnection(settings.DocumentElement["SQLSettings"].ChildNodes);

            var discordConfig = BuildBotConfig(settings.DocumentElement["DiscordConfig"].ChildNodes);
            
            Client = new DiscordSocketClient(discordConfig);
            Commands = new CommandService();
            
            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();
            
            Client.Log += _client_log;
            LoadDiscordEventHandlers();
            
            await RegisterCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, settings.SelectSingleNode("config/BotToken").InnerText);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Register all the Discord Event Hooks
        /// </summary>
        private void LoadDiscordEventHandlers()
        {
            DiscordEventHooks.HookClientEvents(Client);
            DiscordEventHooks.HookMessageEvents(Client);
            DiscordEventHooks.HookMemberEvents(Client);
            DiscordEventHooks.HookChannelEvents(Client);
            DiscordEventHooks.HookBanEvents(Client);
        }

        /// <summary>
        /// Detect the OS and build all OS based variables
        /// </summary>
        private int _detectOS()
        {
            return (int)Environment.OSVersion.Platform;
        }

        /// <summary>
        /// Get the config file location based on the enviroment
        /// </summary>
        /// <param name="enviroment"></param>
        private void _setWorkingDirectory(int enviroment)
        {
            switch (enviroment)
            {
                case 4: //Location of the Linux Config
                    WorkingDirectory = Environment.CurrentDirectory;
                    Console.WriteLine(WorkingDirectory);
                    break;
                case 2: //Location of the Windows Config
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                       "\\.discordtestbot";
                    Console.WriteLine(WorkingDirectory);
                    break;
            }
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
                }
            }

            return new DiscordSocketConfig
            {
                ExclusiveBulkDelete = exclusiveBulkDelete,
                MessageCacheSize = messageCacheSize
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _client_log(LogMessage arg)
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
            Client.MessageReceived += HandleCommandAsync;
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
            var context = new SocketCommandContext(Client, message);
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