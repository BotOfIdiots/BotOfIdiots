using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    internal class Program
    {
        public static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private string _configPath;
        public static IConfiguration Config;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            if ((int) Environment.OSVersion.Platform == 4)
            {
                _configPath = "/etc/DiscordBot/";
            }
            else if ((int) Environment.OSVersion.Platform == 2)
            {
                _configPath = "D:/config/";
            }

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            var _builder = new ConfigurationBuilder()
                .SetBasePath(_configPath)
                .AddJsonFile(path: "config.json");            
            Config = _builder.Build();
            
            _client.Log += _client_log;
            
            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, Config["Token"]);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task _client_log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("$", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if(!result.IsSuccess) Console.WriteLine(result.Error);
            }
        }
    }
}


