using System;
using System.Data.SqlTypes;
using Discord;
using Discord.WebSocket;
using DiscordBot.Modules;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public static class DbOperations
    {
        #region Database Checks

        public static bool CheckJoinRole()
        {
            bool check = false;
            //ulong guildId
            string query = "SELECT JoinRole FROM guild_configurations WHERE Guild = @Guild";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            // guild.Value = guildId;

            // if ()
            return check;
        }

        #endregion

        #region Database Inserts

        public static void InsertUser(ulong userId, ulong guildId)
        {
            string query = "INSERT INTO users (Guild, Snowflake) VALUES (@Guild, @Snowflake)";

            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64);
            snowflake.Value = userId;

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            DiscordBot.DbConnection.ExecuteNonQuery(query, guild, snowflake);
        }

        #endregion

        #region Database Selects

        public static ulong GetLogChannel(string logType, ulong guildId)
        {
            string query = "SELECT " + logType + " FROM log_channels_settings WHERE Guild = @Guild";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            #endregion

            try
            {
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
                MySqlDataReader reader = ExecuteReader(conn, query, guild);

                while (reader.Read())
                {
                    return reader.GetUInt64(logType);
                }
            }

            #region Exception Handlers

            catch (MySqlException ex)
            {
            }
            catch (SqlNullValueException)
            {
            }
            catch (Exception ex)
            {
                EventHandlers.LogException(ex, guildId);
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
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
                MySqlDataReader reader = ExecuteReader(conn, query, guild);

                while (reader.Read())
                {
                    return socketGuild.GetRole(reader.GetUInt64("MutedRole"));
                }
            }
            #region Exception Handlers

            catch (MySqlException ex)
            {
            }
            catch (SqlNullValueException)
            {
            }
            catch (Exception ex)
            {
                EventHandlers.LogException(ex, socketGuild.Id);
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

        public static MySqlDataReader ExecuteReader(MySqlConnection conn, string query,
            params MySqlParameter[] parameters)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteReader();
            }
        }
        #endregion
    }
}