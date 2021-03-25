using System;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using LiteDB;

namespace DiscordBot

{
    public static class ViolationManager
    {
        /// <summary>
        /// Create a New violation insert it into the database and return an embed
        /// </summary>
        /// <param name="violationType"> Violation type. 1 = Ban, 2 = Kick, 3 = mute, 4 = warn </param>
        /// <param name="violator">User that committed the violation</param>
        /// <param name="reason">Reason for the violation</param>
        /// <param name="context">Command Context</param>
        /// <returns>Embed</returns>
        public static Embed NewViolation(SocketGuildUser violator, string reason, SocketCommandContext context,
            int violationType = 0)

        {
            try
            {
                DateTime date = DateTime.Now;

                Violation violation = new Violation
                {
                    UserId = violator.Id,
                    ModeratorId = context.User.Id,
                    Type = violationType,
                    Reason = reason,
                    Date = date
                };


                InsertViolation(violation);

                violation = GetCreatedRecord(date);

                return ViolationEmbed(violation, context);
            }

            catch (IndexOutOfRangeException)
            {
                Embed error = new EmbedBuilder
                    {
                        Title = "Te weinig argumenten"
                    }
                    .WithDescription("Commando is uitgevoerd met te weinig argumenten")
                    .AddField("Example", DiscordBot.Config["CommandPrefix"] + "ban [user] {reason}")
                    .Build();

                return error;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Embed error = new EmbedBuilder
                    {
                        Title = "Exception"
                    }
                    .WithDescription(Format.Sanitize(e.ToString()))
                    .AddField("Time", DateTime.Now)
                    .Build();

                return error;
            }
        }

        /// <summary>
        /// Returns the total ammount of violations for a specified user
        /// </summary>
        /// <param name="userId">id of the user to return the violation count of</param>
        /// <returns>int</returns>
        public static int CountUserViolations(ulong userId)
        {
            using (var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                return table.Count(x => x.UserId == userId);
            }
        }

        /// <summary>
        /// Create a List of Violations commited by user
        /// </summary>
        /// <param name="user">The user of which to return the violations</param>
        /// <returns>List<Violations></returns>
        public static List<Violation> GetViolations(ulong user)
        {
            List<Violation> violations = new List<Violation>();

            using (var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");
                IEnumerable<Violation> queryData = table.Find(x => x.UserId == user);

                foreach (Violation record in queryData)
                {
                    violations.Add(record);
                }
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
            Violation violation = GetViolationRecord(id);

            return ViolationEmbed(violation, context);
        }


        /// <summary>
        /// This functions returns an embed with the information of the committed violation
        /// </summary>
        /// <param name="violation">The violation information</param>
        /// <param name="context">The context of the command that wants the Embed</param>
        /// <returns></returns>
        private static Embed ViolationEmbed(Violation violation, SocketCommandContext context)
        {
            String violationTitle;
            switch (violation.Type)
            {
                case 1:
                    violationTitle = "Banned";
                    break;
                case 2:
                    violationTitle = "Kicked";
                    break;
                case 3:
                    violationTitle = "Muted";
                    break;
                case 4:
                    violationTitle = "Unmuted";
                    break;
                default:
                    violationTitle = "Warned";
                    break;
            }

            Embed embed = new EmbedBuilder
                {
                    Title = violationTitle,
                    Color = Color.Red
                }
                .WithAuthor(context.Client.CurrentUser)
                .AddField("User:", "<@!" + violation.UserId + ">", true)
                .AddField("Date:", DateTime.Now, true)
                .AddField("Moderator:", context.User.Mention)
                .AddField("Reason:", violation.Reason)
                .AddField("Violation ID:", violation.Id, true)
                .WithCurrentTimestamp()
                .WithFooter("UserID: " + violation.UserId)
                .Build();

            return embed;
        }

        /// <summary>
        /// Insert a Violation object into the database
        /// </summary>
        /// <param name="record">The object to insert</param>
        public static void InsertViolation(Violation record)
        {
            using (var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                table.Insert(record);
            }
        }

        /// <summary>
        /// Delete a violation
        /// </summary>
        /// <param name="violationId">Id of violation to delete</param>
        public static void DeleteViolationRecord(int violationId)
        {
            using (var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                table.Delete(violationId);
            }
        }

        /// <summary>
        /// Return a Database record based on the specified date
        /// </summary>
        /// <param name="date"> Date of created record to return</param>
        /// <returns></returns>
        public static Violation GetCreatedRecord(DateTime date)
        {
            using (var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                return table.FindOne(x => x.Date == date);
            }
        }

        /// <summary>
        /// Return a Database record bases on a specified Identifier
        /// </summary>
        /// <param name="id">identifier of the record to return</param>
        /// <returns></returns>
        public static Violation GetViolationRecord(int id)
        {
            using (var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                return table.FindOne(x => x.Id == id);
            }
        }
    }
}