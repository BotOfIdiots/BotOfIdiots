using System;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public static class DbOperations
    {
        public static bool CheckJoinRole()
        {
            bool check = false;
            
            return check;
        }

        public static void InsertUser(ulong userId, ulong guildId)
        {
            string query = "INSERT INTO users (Guild, Snowflake) VALUES (@Guild, @Snowflake)";
            
            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64);
            snowflake.Value = userId;

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            DiscordBot.DbConnection.ExecuteNonQuery(query, guild, snowflake);
        }

        public static ulong GetLogChannel(string logType, ulong guildId)
        {
            string query = "SELECT Logs FROM log_channels_settings WHERE Guild = @Guild";
            
            Console.WriteLine(query);
            
            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            MySqlDataReader dataReader = DiscordBot.DbConnection.ExecuteReader(query, guild);

            Console.WriteLine(dataReader[0].ToString());

            return 0;
        }
    }
}