using System;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Database;
using DiscordBot.Modules;
using DiscordBot.Objects;
using DiscordBot.Objects.Embeds;
using MySql.Data.MySqlClient;

namespace DiscordBot

{
    public static class ViolationManager
    {
        #region New Violation Creation

        /// <summary>
        /// Create a New violation insert it into the database and return an embed
        /// </summary>
        /// <param name="databaseService"></param>
        /// <param name="violationType"> Violation type. 1 = Ban, 2 = Kick, 3 = mute, 4 = warn </param>
        /// <param name="violator">User that committed the violation</param>
        /// <param name="reason">Reason for the violation</param>
        /// <param name="context">Command Context</param>
        /// <param name="confidential">Is it a confidential violation? If yes </param>
        /// <returns>Embed</returns>
        public static Embed NewViolation(SocketGuildUser violator, string reason, ShardedCommandContext context,
            DatabaseService databaseService,
            int violationType = 0, bool confidential = false)

        {
            Violation newViolation = new Violation(context.Client, databaseService, violator.Guild.Id)
            {
                User = violator.Id,
                Moderator = context.User.Id,
                Confidential = confidential,
                Type = violationType,
                Reason = reason,
                Date = DateTime.Now
            };

            newViolation.Insert();

            return new ViolationEmbedBuilder(newViolation, context.Client).Build();
        }

        #endregion

        #region Violation Management

        /// <summary>
        /// Returns the total ammount of violations for a specified user
        /// </summary>
        /// <param name="userId">id of the user to return the violation count of</param>
        /// <param name="guildId"></param>
        /// <param name="databaseService"></param>
        /// <returns>int</returns>
        public static int CountUserViolations(ulong userId, ulong guildId, DatabaseService databaseService,
            DiscordShardedClient client)
        {
            string query = "SELECT count(*) FROM violations WHERE Guild = @Guild AND User = @User";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            MySqlParameter user = new MySqlParameter("@User", MySqlDbType.UInt64);
            user.Value = userId;

            try
            {
                databaseService.CheckConnection();
                using MySqlConnection conn = databaseService.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild, user);

                while (reader.Read())
                {
                    return reader.GetInt32("count(*)");
                }
            }
            catch (Exception e)
            {
                EventHandlers.LogException(e,
                    client.GetGuild(guildId));
            }

            return 0;
        }

        /// <summary>
        /// Create a List of Violations commited by user
        /// </summary>
        /// <param name="user">The user of which to return the violations</param>
        /// <returns>List<Violations></returns>
        public static List<Violation> GetViolations(ulong userId, ulong guildId, DatabaseService databaseService,
            DiscordShardedClient client)
        {
            List<Violation> violations = new List<Violation>();

            string query =
                "SELECT CAST(ViolationId as VARCHAR(6)) as ViolationId FROM violations WHERE Guild = @Guild AND User = @User";

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = guildId;

            MySqlParameter user = new MySqlParameter("@User", MySqlDbType.UInt64);
            user.Value = userId;

            try
            {
                databaseService.CheckConnection();
                using MySqlConnection conn = databaseService.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild, user);

                List<int> violationIds = new List<int>();

                while (reader.Read())
                {
                    violationIds.Add(reader.GetInt32("ViolationId"));
                }

                reader.Close();

                foreach (int id in violationIds)
                {
                    Console.WriteLine(id);
                    violations.Add(Violation.Select(guildId, id, databaseService, client));
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
        public static Embed GetViolation(int id, ShardedCommandContext context, DatabaseService databaseService)
        {
            Violation violation = Violation.Select(context.Guild.Id, id, databaseService, context.Client);

            return new ViolationEmbedBuilder(violation, context.Client).Build();
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