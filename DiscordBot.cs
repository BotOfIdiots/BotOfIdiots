using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
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

        private static IServiceProvider _services;
        public static BotService BotService;
        public static DiscordSocketClient Client;
        public static CommandService Commands;
        private IConfiguration _config;


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RunBotAsync()
        {
            string workingDirectory = _detectOS();
            _createConfig(workingDirectory);
            
            var discordConfig = new DiscordSocketConfig
            {
                MessageCacheSize = Convert.ToInt32(_config.GetSection("DiscordBot")["MessageCacheSize"]),
                ExclusiveBulkDelete = Convert.ToBoolean(_config.GetSection("DiscordBot")["AllowBulkDelete"])
            };

            Client = new DiscordSocketClient(discordConfig);
            BotService = new BotService(_config.GetSection("DiscordBot"), workingDirectory);
            Commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Client.Log += _client_log;
            LoadDiscordEventHandlers();

            await RegisterCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, BotService.Config["Token"]);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        /// <summary>
        /// Detect the OS and build all OS based variables
        /// </summary>
        private string _detectOS()
        {
            int environment = (int) Environment.OSVersion.Platform;
            return _WorkingDirectory(environment);
        }

        /// <summary>
        /// Get the config file location based on the enviroment
        /// </summary>
        /// <param name="enviroment"></param>
        private string _WorkingDirectory(int enviroment)
        {
            string workingDirectory = "";
            switch (enviroment)
            {
                case 4: //Location of the Linux Config
                    workingDirectory = Environment.CurrentDirectory;

                    break;
                case 2: //Location of the Windows Config
                    workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                       "\\.discordtestbot";
                    break;
            }

            if (workingDirectory != "")
            {
                Console.WriteLine(workingDirectory);
                return workingDirectory;
            }
            
            throw new NullReferenceException();
        }

        /// <summary>
        /// Create the config object based on the config.json file
        /// </summary>
        private void _createConfig(string workingDirectory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(workingDirectory)
                .AddJsonFile(path: "config.json");
            _config = builder.Build();
        }

        /// <summary>
        /// Register all the Discord Event Hooks
        /// </summary>
        private void LoadDiscordEventHandlers()
        {
            DiscordEventHooks.HookMessageDeleted(Client);
            DiscordEventHooks.HookMessageBulkDeleted(Client);
            DiscordEventHooks.HookMessageUpdated(Client);
            DiscordEventHooks.HookMemberJoinGuild(Client);
            DiscordEventHooks.HookMemberLeaveGuild(Client);
            DiscordEventHooks.HookMemberVoiceState(Client);
            DiscordEventHooks.HookMemberUpdated(Client);
            DiscordEventHooks.HookMemberBanned(Client);
            DiscordEventHooks.HookMemberUnbanned(Client);
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
        private async Task RegisterCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
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
            }
        }
    }
}