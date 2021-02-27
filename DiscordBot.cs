using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    internal class DiscordBot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) => new DiscordBot().RunBotAsync().GetAwaiter().GetResult();

        private static readonly string _version = "0.0.2";
        private static IServiceProvider _services;
        private string _configPath;
        public static DiscordSocketClient Client;
        public static CommandService Commands;
        public static IConfiguration Config;
        public static ulong GuildId;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RunBotAsync()
        {
            _detectOS();
            _createConfig();
            
            var discordConfig = new DiscordSocketConfig
            {
                MessageCacheSize = Convert.ToInt32(Config["MessageCacheSize"]),
                ExclusiveBulkDelete = Convert.ToBoolean(Config["AllowBulkDelete"])
            };
            
            Client = new DiscordSocketClient(discordConfig);
            Commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();
            
            Client.Log += _client_log;
            LoadDiscordEventHandlers();
            
            
            await RegisterCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, Config["Token"]);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Register all the Discord Event Hooks
        /// </summary>
        private void LoadDiscordEventHandlers()
        {
            DiscordEventHandler.HookMessageDeleted(Client);
            DiscordEventHandler.HooMessageBulkDelted(Client);
            DiscordEventHandler.HookMessageUpdated(Client);
            DiscordEventHandler.HookMemberJoinGuild(Client);
            DiscordEventHandler.HookMemberLeaveGuild(Client);
            DiscordEventHandler.HookMemberVoiceState(Client);
            DiscordEventHandler.HookMemberUpdated(Client);
            DiscordEventHandler.HookMemberBanned(Client);
            DiscordEventHandler.HookMemberUnbanned(Client);
        }

        /// <summary>
        /// Detect the OS and build all OS based variables
        /// </summary>
        private void _detectOS()
        {
            int environment = (int) Environment.OSVersion.Platform;
            _getConfigPath(environment);
            
        }

        /// <summary>
        /// Get the config file location based on the enviroment
        /// </summary>
        /// <param name="enviroment"></param>
        private void _getConfigPath(int enviroment)
        {
            switch (enviroment)
            {
                case 4: //Location of the Linux Config
                    _configPath = "/home/botofidiots/";
                    break;
                case 2: //Location of the Windows Config
                    _configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                  "/.discordtestbot";
                    break;
            }
        }
        
        /// <summary>
        /// Create the config object based on the config.json file
        /// </summary>
        private void _createConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(_configPath)
                .AddJsonFile(path: "config.json");
            Config = builder.Build();
            GuildId = Convert.ToUInt64(Config["GuildId"]);
        }

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
            if (message.HasStringPrefix(Config["CommandPrefix"], ref argPos))
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
            }
        }
    }
}