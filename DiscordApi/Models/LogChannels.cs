using System;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.DiscordApi.Models
{
    public class LogChannels
    {
        private readonly SocketGuild _socketGuild = Base.DiscordBot.Client.GetGuild(Base.DiscordBot.GuildId); 
        public SocketTextChannel Logs { get; }
        public SocketTextChannel JoinLeave { get; }
        public SocketTextChannel Punishments{ get; }
        public SocketTextChannel Messages { get; }
        public SocketTextChannel Voice { get; }
        public SocketTextChannel Roles { get; }
        public SocketTextChannel Nickname { get; }
        public SocketTextChannel ChannelUpdates { get; }
        public SocketTextChannel Commands { get; }
        public SocketTextChannel Exceptions { get; }
        

        public LogChannels(IConfigurationSection config)
        {
            Logs = SetChannel(config["Logs"]);
            JoinLeave = SetChannel(config["JoinLeave"]);
            Punishments = SetChannel(config["Punishments"]);
            Messages = SetChannel(config["Messages"]);
            Voice = SetChannel(config["Voice"]);
            Roles = SetChannel(config["Roles"]);
            Nickname = SetChannel(config["Nickname"]);
            ChannelUpdates = SetChannel(config["ChannelUpdates"]);
            Commands = SetChannel(config["Commands"]);
            Exceptions = SetChannel(config["Exceptions"]);
        }

        /// <summary>
        /// Converts id from config file to a SocketTextChannel that is useable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private SocketTextChannel SetChannel(string id)
        {
            return _socketGuild.GetTextChannel(Convert.ToUInt64(id));
        }
       
    }
}