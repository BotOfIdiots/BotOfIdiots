using System;
using System.Data.SqlTypes;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules;
using DiscordBot.Modules.Event;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public static class DbOperations
    {
        private static readonly DatabaseService DatabaseService = DiscordBot.Services.GetRequiredService<DatabaseService>();
        
        #region Database Checks

        public static bool CheckJoinRole(SocketGuild socketGuild)
        {
            bool check = false;
            string query = "SELECT JoinRole FROM guild_configurations WHERE Guild = @Guild";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = socketGuild.Id;

            // if ()
            return check;
        }

        public static bool CheckPrivateChannel(SocketGuild socketGuild)
        {
            String query = "SELECT CategoryId FROM private_channels_setups WHERE Guild = @Guild;";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };

            using MySqlDataReader reader = ExecuteReader(DatabaseService, query, guild);

            while (reader.Read())
            {
                if (reader.GetUInt64("CategoryId") != 0) return true;
            }

            return false;
        }

        public static bool CheckLogExemption(SocketGuildChannel socketGuildChannel)
        {
            String query = "SELECT Snowflake FROM exclude_from_logging WHERE Guild = @Guild AND Snowflake = @Channel";

            #region Mysql Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64)
                { Value = socketGuildChannel.Guild.Id };
            
            MySqlParameter channel = new MySqlParameter("@Channel", MySqlDbType.UInt64)
                { Value = socketGuildChannel.Id };

            #endregion
            
            using MySqlDataReader reader = ExecuteReader(DatabaseService, query, guild, channel);

            while (reader.Read())
            {
                if (reader.GetUInt64("Snowflake") == socketGuildChannel.Id) return true;
            }

            return false;
        }

        #endregion

        #region Database Inserts

        public static bool InsertUser(ulong userId, SocketGuild socketGuild)
        {
            string query = "INSERT INTO users (Guild, Snowflake) VALUES (@Guild, @Snowflake)";

            #region SQL Parameters

            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64);
            snowflake.Value = userId;

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = socketGuild.Id;

            #endregion

            int result = DatabaseService.ExecuteNonQuery(query, guild, snowflake);

            if (result == 1) return true;
            
            return false;
        }

        public static bool InsertReactionMessage(ulong messageSnowflake, SocketGuild socketGuild)
        {
            string query = "INSERT INTO reaction_messages (Guild, MessageSnowflake) VALUE (@Guild, @Snowflake);";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };

            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64)
                { Value = messageSnowflake };

            #endregion

            int result = DatabaseService.ExecuteNonQuery(query, guild, snowflake);

            if (result == 1) return true;

            return false;
        }

        #endregion

        #region Database Selects

        public static ulong GetLogChannel(string logType, SocketGuild socketGuild)
        {
            string query = $"SELECT {logType} FROM log_channels_settings WHERE Guild = @Guild";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = socketGuild.Id;

            #endregion

            try
            {
                
                using MySqlDataReader reader = ExecuteReader(DatabaseService, query, guild);

                while (reader.Read())
                {
                    return reader.GetUInt64(logType);
                }
            }

            #region Exception Handlers

            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
            }
            catch (SqlNullValueException)
            {
            }
            catch (Exception ex)
            {
                EventHandlers.LogException(ex, socketGuild);
            }

            #endregion

            return 0;
        }

        public static SocketRole GetMutedRole(SocketGuild socketGuild)
        {
            string query = "SELECT MutedRole FROM guild_configurations WHERE Guild = @Guild";

            #region parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };

            #endregion

            try
            {
                using MySqlDataReader reader = ExecuteReader(DatabaseService, query, guild);

                while (reader.Read())
                {
                    ulong role = reader.GetUInt64("MutedRole");

                    EventHandlers.LogException(new Exception($"Role id: {role}"), socketGuild);
                    return socketGuild.GetRole(role);
                }
            }

            #region Exception Handlers

            catch (MySqlException ex)
            {
                Console.WriteLine(ex.ToString());
                EventHandlers.LogException(ex, socketGuild);
            }
            catch (Exception ex)
            {
                EventHandlers.LogException(ex, socketGuild);
            }

            #endregion

            return null;
        }

        public static Object ExecuteScalar(MySqlConnection conn, string query,
            params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }

        public static MySqlDataReader ExecuteReader(DatabaseService databaseService, string query,
            params MySqlParameter[] parameters)
        {
            DatabaseService.CheckConnection();
            using (MySqlCommand cmd = DatabaseService.SqlConnection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteReader();
            }
        }

        #endregion

        public static IRole GetJoinRole()
        {
            throw new NotImplementedException();
        }
    }
}