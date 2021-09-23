using System;
using System.Collections.Generic;
using System.Data;
using Discord.WebSocket;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public class JoinedGuild
    {
        public static void AddGuild(SocketGuild guild)
        {
            string query = "INSERT INTO guilds (Snowflake, GuildName) VALUES (@Snowflake, @GuildName)";

            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64);
            snowflake.Value = guild.Id;

            MySqlParameter guildName = new MySqlParameter("@Guildname", MySqlDbType.VarChar);
            guildName.Value = guild.Name;

            DiscordBot.DbConnection.ExecuteNonQuery(query, snowflake, guildName);
            
            string guildConfigurationQuery = "INSERT INTO guild_configurations (Guild) VALUES (@Snowflake)";
            DiscordBot.DbConnection.ExecuteNonQuery(guildConfigurationQuery, snowflake);
        }

        public static void DownloadMembers(IReadOnlyCollection<SocketGuildUser> UserList, ulong GuildId)
        {
            string query = "INSERT INTO users (Guild, Snowflake) VALUES (@Guild, @Snowflake)";
            MySqlParameter snowflake = new MySqlParameter("@Snowflake", MySqlDbType.UInt64);

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = GuildId;

            foreach (SocketGuildUser user in UserList)
            {
                snowflake.Value = user.Id;
                DiscordBot.DbConnection.ExecuteNonQuery(query, guild, snowflake);
            }
        }

        public static void SetGuildOwner(ulong GuildOwner)
        {
            string query = "UPDATE guilds SET GuildOwner = @GuildOwner";

            MySqlParameter guildOwner = new MySqlParameter("@GuildOwner", MySqlDbType.UInt64);
            guildOwner.Value = GuildOwner;

            DiscordBot.DbConnection.ExecuteNonQuery(query, guildOwner);
        }
    }
}