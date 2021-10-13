using System;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Database;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Models
{
    public static class LogChannels
    {
        #region Moderation Log Channels
        public static SocketTextChannel Logs(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Logs", guild);
            try
            {
                return DiscordBot.Client.GetGuild(guild).GetTextChannel(channelSnowflake);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public static SocketTextChannel JoinLeave(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("JoinLeave", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Punishments(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Punishments", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Messages (ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Messages", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Voice(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Voice", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Roles(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Roles", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Nickname(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Nickname", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        #endregion
        
        #region Admin Log Channels
        public static SocketTextChannel ChannelUpdates(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("ChannelUpdates", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Commands(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Commands", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Exceptions(ulong guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Exceptions", guild);
            try
            {
                return DiscordBot.Client.GetGuild(guild).GetTextChannel(channelSnowflake);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        #endregion

        #region Methods
        private static SocketTextChannel CreateLogChannel(ulong channel, ulong guild)
        {
            if (channel != 0)
            {
                return DiscordBot.Client.GetGuild(guild).GetTextChannel(channel);
            }
            return Logs(guild);
        }
        #endregion
    }
}