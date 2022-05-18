using System;
using Discord.WebSocket;
using DiscordBot.Database;

namespace DiscordBot.Modules.Logging
{
    public static class LogChannels
    {
        #region Moderation Log Channels
        //TODO Change ulong Guild to socketGuild
        public static SocketTextChannel Logs(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Logs", guild);
            try
            {
                return guild.GetTextChannel(channelSnowflake);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        public static SocketTextChannel JoinLeave(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("JoinLeave", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Punishments(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Punishments", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Messages (SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Messages", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Voice(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Voice", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Roles(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Roles", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Nickname(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Nickname", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        #endregion
        
        #region Admin Log Channels
        public static SocketTextChannel ChannelUpdates(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("ChannelUpdates", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Commands(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Commands", guild);
            return CreateLogChannel(channelSnowflake, guild);
        }
        public static SocketTextChannel Exceptions(SocketGuild guild)
        {
            ulong channelSnowflake = DbOperations.GetLogChannel("Exceptions", guild);
            try
            {
                return guild.GetTextChannel(channelSnowflake);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        #endregion

        #region Methods
        private static SocketTextChannel CreateLogChannel(ulong channel, SocketGuild guild)
        {
            if (channel != 0)
            {
                return guild.GetTextChannel(channel);
            }
            return Logs(guild);
        }
        #endregion
    }
}