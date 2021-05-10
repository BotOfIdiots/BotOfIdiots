using System;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Models
{
    public class LogChannels
    {
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
        

        public LogChannels()
        {
            IConfiguration config = DiscordBot.BotService.Config.GetSection("LogChannels");
            SocketGuild socketGuild = DiscordBot.Client.GetGuild(DiscordBot.BotService.GuildId); 
            
            Logs = socketGuild.GetTextChannel(Convert.ToUInt64(config["Logs"]));
            JoinLeave = socketGuild.GetTextChannel(Convert.ToUInt64(config["JoinLeave"]));
            Punishments = socketGuild.GetTextChannel(Convert.ToUInt64(config["Punishments"]));
            Messages = socketGuild.GetTextChannel(Convert.ToUInt64(config["Messages"]));
            Voice = socketGuild.GetTextChannel(Convert.ToUInt64(config["Voice"]));
            Roles = socketGuild.GetTextChannel(Convert.ToUInt64(config["Roles"]));
            Nickname = socketGuild.GetTextChannel(Convert.ToUInt64(config["Nickname"]));
            ChannelUpdates = socketGuild.GetTextChannel(Convert.ToUInt64(config["ChannelUpdates"]));
            Commands = socketGuild.GetTextChannel(Convert.ToUInt64(config["Commands"]));
            Exceptions = socketGuild.GetTextChannel(Convert.ToUInt64(config["Exceptions"]));
            
        }
       
    }
}