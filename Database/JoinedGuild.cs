using System;
using System.Collections.Generic;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public class JoinedGuild
    {
        public static void AddGuild(SocketGuild guild, IServiceProvider services)
        {
            DatabaseService databaseService = services.GetRequiredService <DatabaseService>();
            
            if (!GuildConfigExists(guild, databaseService))
            {
                string query = "INSERT INTO guilds (Snowflake, GuildName) VALUES (@Snowflake, @GuildName)";

                #region SQL Parameters

                MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64)
                {
                    Value = guild.Id
                };

                MySqlParameter guildName = new MySqlParameter("@Guildname", MySqlDbType.VarChar)
                {
                    Value = guild.Name
                };
                
                #endregion
                
                databaseService.ExecuteNonQuery(query, snowflake, guildName);
                
                string guildConfigurationQuery = "INSERT INTO guild_configurations (Guild) VALUES (@Snowflake)";
                databaseService.ExecuteNonQuery(guildConfigurationQuery, snowflake);
            }
        }

        private static bool GuildConfigExists(SocketGuild socketGuild, DatabaseService databaseService)
        {
            string query = "SELECT Guild FROM guild_configurations WHERE Guild = @Guild";
            
            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };

           databaseService.CheckConnection();
            MySqlConnection conn = databaseService.SqlConnection;
            MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild);

            while (reader.Read())
            {
                if (reader.GetUInt64("Guild") == socketGuild.Id)
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();

            return false;
        }

        public static void DownloadMembers(IReadOnlyCollection<SocketGuildUser> UserList, ulong GuildId, IServiceProvider services)
        {
            DatabaseService databaseService = services.GetRequiredService <DatabaseService>();
            
            string query = "INSERT INTO users (Guild, Snowflake) VALUES (@Guild, @Snowflake)";

            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64);

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = GuildId };

            foreach (SocketGuildUser user in UserList)
            {
                snowflake.Value = user.Id;
                databaseService.ExecuteNonQuery(query, guild, snowflake);
            }
        }

        public static void SetGuildOwner(ulong GuildOwner, ulong GuildId, IServiceProvider services)
        {
            DatabaseService databaseService = services.GetRequiredService <DatabaseService>();
            
            string query = "UPDATE guilds SET GuildOwner = @GuildOwner WHERE Snowflake = @GuildId";

            #region SQL Parameters

            MySqlParameter guildId = new MySqlParameter("@GuildId", MySqlDbType.UInt64) { Value = GuildId };

            MySqlParameter guildOwner = new MySqlParameter("@GuildOwner", MySqlDbType.UInt64) { Value = GuildOwner };

            #endregion

            databaseService.ExecuteNonQuery(query, guildOwner, guildId);
        }

        public static void GenerateDefaultViolation(SocketGuild socketGuild, IServiceProvider services)
        {
            DatabaseService databaseService = services.GetRequiredService<DatabaseService>();
            SocketUser socketUser = services.GetRequiredService<DiscordShardedClient>().CurrentUser;
            
            string query =
                "INSERT INTO violations (Guild, ViolationId, User, Moderator, Type, Reason, Date) VALUE (@Guild, 0, @User, @User, 0, 'Default violation for initialization', @Date);";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = socketGuild.Id };
            MySqlParameter user = new MySqlParameter("@User", MySqlDbType.UInt64) { Value = socketUser.Id };
            MySqlParameter date = new MySqlParameter("@Date", MySqlDbType.DateTime) { Value = DateTime.Now };

            #endregion

            databaseService.ExecuteNonQuery(query, guild, user, date);
        }
    }
}