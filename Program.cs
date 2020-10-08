using System;
using System.Net.WebSockets;
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

        private static string _version = "0.0.1";
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private string _configPath;
        public static IConfiguration Config;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            
            //Checks OS version to determine te location of the Config file.
            if ((int) Environment.OSVersion.Platform == 4) //Location of the Linux Config
            {
                _configPath = "/home/botofidiots/";
            }
            else if ((int) Environment.OSVersion.Platform == 2) //Location of the Windows Config
            {
                _configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.discordtestbot";
            }

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            //Get the config options
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

        //Returns the Version String
        public static string Version()
        {
            return _version;
        }
        
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix(Config["CommandPrefix"], ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if(!result.IsSuccess) Console.WriteLine(result.Error);
            }
        }
    }
}


