using System;
using System.Threading;
using DiscordBot.DiscordApi;
using DiscordBot.Twitch;
using Microsoft.Extensions.Configuration;
namespace DiscordBot
{
    internal static class Base
    {
        private static readonly string _version = "0.0.5 Twitch Integration";
        
        public static Thread DiscordModule;
        public static Bot DiscordBot;

        private static bool TwitchEnabled;
        public static Thread TwitchModule;
        public static TwitchIntegration TwitchIntegration;
        
        public static string WorkingDirectory;
        public static IConfiguration Config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            _detectOS();
            _createConfig();
            _loadModules();

            DiscordBot = new Bot(Config.GetSection("DiscordBot"));
            TwitchIntegration = new TwitchIntegration(Config.GetSection("Twitch"));

            DiscordModule = new Thread(DiscordThread);
            DiscordModule.Start();
            
            if (TwitchEnabled)
            {
                Thread TwitchModule = new Thread(TwitchThread);
                TwitchModule.Start();
            }
        }

        private static void DiscordThread()
        {
            Console.WriteLine("[Discord:] Starting DiscordBot Module");
            DiscordBot.StartBotAsync().GetAwaiter().GetResult();
        }

        private static void TwitchThread()
        {
            try
            {
                while (true)
                {
                    if (DiscordBot.Ready)
                    {
                        Console.WriteLine("[Twitch:] Starting Twitch Module");
                        TwitchIntegration.StartAsync();
                        break;
                    }
                    Console.WriteLine("[Twitch:] Waiting for Discordbot to come online");
                    Thread.Sleep(10 * 1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            

        }
        
        private static void _loadModules()
        {
            TwitchEnabled = Convert.ToBoolean(Config.GetSection("Twitch")["Enabled"]);
        }
        
        /// <summary>
        /// Detect the OS and build all OS based variables
        /// </summary>
        private static void _detectOS()
        {
            int environment = (int) Environment.OSVersion.Platform;
            _setWorkingDirectory(environment);
        }

        /// <summary>
        /// Get the config file location based on the enviroment
        /// </summary>
        /// <param name="enviroment"></param>
        private static void _setWorkingDirectory(int enviroment)
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
        private static void _createConfig()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(WorkingDirectory)
                    .AddJsonFile(path: "config.json");
                Config = builder.Build();
            }

            // catch (FileNotFoundException)
            // {
            //
            // }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Version()
        {
            return _version;
        }
    }
}