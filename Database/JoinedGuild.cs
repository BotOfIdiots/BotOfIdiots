using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Discord.WebSocket;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public class JoinedGuild
    {
        public static void AddGuild(SocketGuild guild)
        {
            string query = "INSERT INTO guilds (Snowflake, GuildName) VALUES (@Snowflake, @GuildName)";

            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64)
            {
                Value = guild.Id
            };

            MySqlParameter guildName = new MySqlParameter("@Guildname", MySqlDbType.VarChar)
            {
                Value = guild.Name
            };

            DiscordBot.DbConnection.ExecuteNonQuery(query, snowflake, guildName);

            string guildConfigurationQuery = "INSERT INTO guild_configurations (Guild) VALUES (@Snowflake)";
            DiscordBot.DbConnection.ExecuteNonQuery(guildConfigurationQuery, snowflake);
        }

        public static void DownloadMembers(IReadOnlyCollection<SocketGuildUser> UserList, ulong GuildId)
        {
            string query = "INSERT INTO users (Guild, Snowflake) VALUES (@Guild, @Snowflake)";
            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64);

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64)
            {
                Value = GuildId
            };

            foreach (SocketGuildUser user in UserList)
            {
                snowflake.Value = user.Id;
                DiscordBot.DbConnection.ExecuteNonQuery(query, guild, snowflake);
            }
        }

        public static void SetGuildOwner(ulong GuildOwner, ulong GuildId)
        {
            string query = "UPDATE guilds SET GuildOwner = @GuildOwner WHERE Snowflake = @GuildId";

            MySqlParameter guildId = new MySqlParameter("@GuildId", MySqlDbType.UInt64)
            {
                Value = GuildId
            };

            MySqlParameter guildOwner = new MySqlParameter("@GuildOwner", MySqlDbType.UInt64)
            {
                Value = GuildOwner
            };

            DiscordBot.DbConnection.ExecuteNonQuery(query, guildOwner, guildId);
        }

        public static void GenerateDefaultViolation(SocketGuild socketGuild, SocketUser socketUser)
        {
            string query =
                "INSERT INTO violations (Guild, ViolationId, User, Moderator, Type, Reason, Date) VALUE (@Guild, 0, @User, @User, 0, 'Default violation for initialization', @Date);";
            
            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };
            MySqlParameter user = new MySqlParameter("@User", MySqlDbType.UInt64) { Value = socketUser.Id };
            MySqlParameter date = new MySqlParameter("@Date", MySqlDbType.DateTime) { Value = DateTime.Now };

            #endregion

            DiscordBot.DbConnection.ExecuteNonQuery(query, guild, user, date);
        }
    }
}