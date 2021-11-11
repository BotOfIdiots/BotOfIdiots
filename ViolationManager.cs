using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Channels;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Models;
using DiscordBot.Models.Embeds;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Digests;

namespace DiscordBot

{
    public static class ViolationManager
    {
        #region New Violation Creation

        /// <summary>
        /// Create a New violation insert it into the database and return an embed
        /// </summary>
        /// <param name="violationType"> Violation type. 1 = Ban, 2 = Kick, 3 = mute, 4 = warn </param>
        /// <param name="violator">User that committed the violation</param>
        /// <param name="reason">Reason for the violation</param>
        /// <param name="context">Command Context</param>
        /// <param name="confidential">Is it a confidential violation? If yes </param>
        /// <returns>Embed</returns>
        public static Embed NewViolation(SocketGuildUser violator, string reason, SocketCommandContext context,
            int violationType = 0, bool confidential = false)

        {
            Violation newViolation = new Violation(violator.Guild.Id)
            {
                User = violator.Id,
                Moderator = context.User.Id,
                Confidential = confidential,
                Type = violationType,
                Reason = reason,
                Date = DateTime.Now
            };
            
            newViolation.Insert();

            return new ViolationEmbedBuilder(newViolation).Build();
            
        }
        #endregion
        
        #region Violation Management
        /// <summary>
        /// Returns the total ammount of violations for a specified user
        /// </summary>
        /// <param name="userId">id of the user to return the violation count of</param>
        /// <returns>int</returns>
        public static int CountUserViolations(ulong userId, ulong guildId)
        {
            string query = "SELECT count(*) FROM violations WHERE Guild = @Guild AND User = @User";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            MySqlParameter user = new MySqlParameter("@User", MySqlDbType.UInt64);
            user.Value = userId;

            try
            {
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild, user);

                while (reader.Read())
                {
                    return reader.GetInt32("count(*)");
                }
            }
            catch
            {
            }

            return 0;
        }

        /// <summary>
        /// Create a List of Violations commited by user
        /// </summary>
        /// <param name="user">The user of which to return the violations</param>
        /// <returns>List<Violations></returns>
        public static List<Violation> GetViolations(ulong userId, ulong guildId)
        {
            List<Violation> violations = new List<Violation>();

            string query = "SELECT CAST(ViolationId as VARCHAR(6)) as ViolationId FROM violations WHERE Guild = @Guild AND User = @User";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            MySqlParameter user = new MySqlParameter("@User", MySqlDbType.UInt64);
            user.Value = userId;

            try
            {
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild, user);

                List<int> violationIds = new List<int>();

                while (reader.Read())
                {
                    violationIds.Add( reader.GetInt32("ViolationId"));
                }
                reader.Close();

                foreach (int id in violationIds)
                {
                    Console.WriteLine(id);
                    violations.Add(Violation.Select(guildId, id));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return violations;
        }


        /// <summary>
        /// Get a specific violation
        /// </summary>
        /// <param name="id">Id of violation to return</param>
        /// <param name="context">Context of issued command</param>
        /// <returns></returns>
        public static Embed GetViolation(int id, SocketCommandContext context)
        {
            Violation violation = Violation.Select(context.Guild.Id, id);

            return new ViolationEmbedBuilder(violation).Build();
        }
        #endregion

        // /// <summary>
        // /// This functions returns an embed with the information of the committed violation
        // /// </summary>
        // /// <param name="violation">The violation information</param>
        // /// <param name="context">The context of the command that wants the Embed</param>
        // /// <returns></returns>
        // private static Embed ViolationEmbed(Violation violation, SocketCommandContext context)
        // {
        //     String violationTitle;
        //
        //
        //     Embed embed = new EmbedBuilder
        //         {
        //             Title = violationTitle,
        //             Color = Color.Red
        //         }
        //         .WithAuthor(context.Client.CurrentUser)
        //         .AddField("User:", "<@!" + violation.UserId + ">", true)
        //         .AddField("Date:", DateTime.Now, true)
        //         .AddField("Moderator:", context.User.Mention)
        //         .AddField("Reason:", violation.Reason)
        //         .AddField("Violation ID:", violation.Id, true)
        //         .WithCurrentTimestamp()
        //         .WithFooter("UserID: " + violation.UserId)
        //         .Build();
        //
        //     return embed;
        // }
    }
}