using System;
using System.Linq;
using System.Net.Http.Headers;
using Discord;
using DiscordBot.Models;
using DiscordBot.Modules;
using LiteDB;
using Microsoft.Extensions.Configuration;


namespace DiscordBot
{
    public class BotService
    {
        public readonly string Version = "0.0.4";
        public readonly IConfiguration Config;
        public readonly LiteDatabase Database;
        public readonly ulong GuildId;
        // public readonly EventHandlers EventHandlers;
        private string _workingDirectory;
        // public readonly IRole MutedRole;


        /// <summary>
        /// 
        /// </summary>
        public BotService(IConfiguration config, string workingDirectory)
        {

            _workingDirectory = workingDirectory;
            Config = config;

            GuildId = Convert.ToUInt64(Config["GuildId"]);

            // LogChannels logChannels = new LogChannels(Config.GetSection("LogChannels"));
            // EventHandlers = new EventHandlers(logChannels);

            Database = new LiteDatabase(_workingDirectory + "/Database.db");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetWorkingDirectory()
        {
            return _workingDirectory;
        }

        
    }
}