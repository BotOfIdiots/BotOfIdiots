using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Discord.WebSocket;
using DiscordBot.DiscordApi;
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

            Bot.DbConnection.ExecuteNonQuery(query, snowflake, guildName);
            
            string guildConfigurationQuery = "INSERT INTO guild_configurations (Guild) VALUES (@Snowflake)";
            Bot.DbConnection.ExecuteNonQuery(guildConfigurationQuery, snowflake);
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
                Bot.DbConnection.ExecuteNonQuery(query, guild, snowflake);
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

            Bot.DbConnection.ExecuteNonQuery(query, guildOwner, guildId);
        }
    }
}