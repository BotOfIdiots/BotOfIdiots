using System;
using System.Linq;
using Discord;
using Discord.WebSocket;
using DiscordBot.Database;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace DiscordBot.Modules.Chat.Class
{
    public static class ReactionRoleMessage
    {
        private static readonly DatabaseService DatabaseService = DiscordBot.Services.GetRequiredService<DatabaseService>();

        public static bool IsReactionMessage(ulong messageId)
        {
            string query = "SELECT MessageSnowflake " +
                           "FROM reaction_messages " +
                           "WHERE MessageSnowflake = @Id";

            MySqlParameter message = new MySqlParameter("@Id", MySqlDbType.UInt64) { Value = messageId };

            using MySqlDataReader reader = DbOperations.ExecuteReader(DatabaseService, query, message);

            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    return true;
                }
            }

            return false;
        }

        public static void AddRole(SocketReaction reaction, ulong message, SocketGuild socketGuild)
        {
            try
            {
                IRole role = socketGuild.GetRole(GetRole(reaction.Emote, message));
                SocketGuildUser user = socketGuild.GetUser(reaction.UserId);

                if (!UserHasRole(user, role))
                {
                    user.AddRoleAsync(role);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Role doesn't exist")
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void RemoveRole(SocketReaction reaction, ulong message, SocketGuild socketGuild)
        {
            try
            {
                IRole role = socketGuild.GetRole(GetRole(reaction.Emote, message));
                SocketGuildUser user = socketGuild.GetUser(reaction.UserId);

                if (!UserHasRole(user, role))
                {
                    user.RemoveRoleAsync(role);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Role doesn't exist")
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static ulong GetRole(IEmote emote, ulong messageId)
        {
            string query =
                "SELECT RoleSnowflake " +
                "FROM reaction_roles r JOIN reaction_messages m on m.Id = r.ReactionMessage " +
                "WHERE MessageSnowflake = @Message AND Reaction = @Reaction";

            MySqlParameter message = new MySqlParameter("@Message", MySqlDbType.UInt64) { Value = messageId };
            MySqlParameter reaction = new MySqlParameter("@Reaction", MySqlDbType.VarChar) { Value = emote.Name };

            using MySqlDataReader reader = DbOperations.ExecuteReader(DatabaseService, query, message, reaction);

            while (reader.Read())
            {
                if (reader.GetUInt64("RoleSnowflake") != 0)
                {
                    return reader.GetUInt64("RoleSnowflake");
                }
            }
            
            throw new Exception("Role doesn't exist");
        }
        
        private static bool UserHasRole(SocketGuildUser user, IRole role)
        {
            if (user.Roles.Contains(role))
            {
                return true;
            }
            return false;
        }

        public static void AddMessage(ulong messageId, ulong guildId)
        {
            string query = "INSERT INTO reaction_messages (Guild, MessageSnowflake) VALUE (@Guild, @Message)";

            MySqlParameter guild = new MySqlParameter("Guild", MySqlDbType.UInt64) { Value = guildId };
            MySqlParameter message = new MySqlParameter("@Message", MySqlDbType.UInt64) { Value = messageId };

            int result = DatabaseService.ExecuteNonQuery(query, guild, message);
            if (result == 0)
            {
                throw new Exception("Couldn't create message");
            }

        }

        public static void RemoveMessage(ulong messageId, ulong guildId)
        {
            string query = "SELECT Id FROM reaction_messages WHERE Guild = @Guild AND MessageSnowflake = @message";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = guildId };
            MySqlParameter message = new MySqlParameter("@message", MySqlDbType.UInt64) { Value = messageId };

            MySqlDataReader reader = DbOperations.ExecuteReader(DatabaseService, query, guild, message);

            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    RemoveAllReactions(reader.GetInt32("Id"));
                    
                    query = "DELETE FROM reaction_messages WHERE Id = @Id";
                    MySqlParameter id = new MySqlParameter("@Id", MySqlDbType.UInt32) { Value = reader.GetInt32("Id") };

                    DatabaseService.ExecuteNonQuery(query, id);
                }
            }
        }

        public static void AddReaction()
        {
            
        }

        public static void RemoveReaction()
        {
            
        }

        private static void RemoveAllReactions(int index)
        {
            string query = "DELETE FROM reaction_roles WHERE ReactionMessage = @Id";

            MySqlParameter id = new MySqlParameter("@Id", MySqlDbType.Int32) { Value = index };

            DatabaseService.ExecuteNonQuery(query, id);
        }
    }
}