using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.DiscordApi
{
    public class Bot
    {
        private IServiceProvider _services;
        private readonly string _apiToken;
        public bool Ready = false;
        public readonly ulong GuildId;
        public IConfiguration Config;
        public DiscordSocketClient Client;
        public CommandService Commands;

        public Bot(IConfiguration discordConfig)
        {
            Config = discordConfig;
            _apiToken = Config["Token"];
            
            GuildId = Convert.ToUInt64(Config["GuildId"]);
            
            var discordSocketConfig = new DiscordSocketConfig
            {
                MessageCacheSize = Convert.ToInt32(Config["MessageCacheSize"]),
                ExclusiveBulkDelete = Convert.ToBoolean(Config["AllowBulkDelete"])
            };
            
            Client = new DiscordSocketClient(discordSocketConfig);
            Commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Client.Log += _client_log;
            
            LoadDiscordEventHooks();
            RegisterCommandsAsync().GetAwaiter();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StartBotAsync()
        {
            try
            {
                await Client.LoginAsync(TokenType.Bot, _apiToken);
                await Client.StartAsync();
                Ready = true;
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task StopBotAsync()
        {
            await Client.StopAsync();
            await Client.LogoutAsync();
        }

        

        /// <summary>
        /// Register all the Discord Event Hooks
        /// </summary>
        private void LoadDiscordEventHooks()
        {
            DiscordEventHooks.HookMessageDeleted(Client);
            DiscordEventHooks.HookMessageBulkDeleted(Client);
            DiscordEventHooks.HookMessageUpdated(Client);
            // DiscordEventHooks.HookMemberJoinGuild(Client);
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
        public async Task RegisterCommandsAsync()
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
            if (message != null && message.Author.IsBot) return;

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