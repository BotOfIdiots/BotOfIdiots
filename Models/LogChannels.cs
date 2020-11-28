using System;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Models
{
    public class LogChannels
    {
        public ulong Logs { get; }
        public ulong JoinLeave { get; }
        public ulong Punishments{ get; }
        public ulong Messages { get; }
        public ulong Voice { get; }
        public ulong Roles { get; }
        public ulong ChannelUpdates { get; }
        public ulong Commands { get; }
        public ulong Exceptions { get; }
        

        public LogChannels(IConfigurationSection config)
        {
            Logs = Convert.ToUInt64(config["Logs"]);
            JoinLeave = Convert.ToUInt64(config["JoinLeave"]);
            Punishments = Convert.ToUInt64(config["Punishments"]);
            Messages = Convert.ToUInt64(config["Messages"]);
            Voice = Convert.ToUInt64(config["Voice"]);
            Roles = Convert.ToUInt64(config["Roles"]);
            ChannelUpdates = Convert.ToUInt64(config["ChannelUpdates"]);
            Commands = Convert.ToUInt64(config["Commands"]);
            Exceptions = Convert.ToUInt64(config["Exceptions"]);
            
        }
       
    }
}