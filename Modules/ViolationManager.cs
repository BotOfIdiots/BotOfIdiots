using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using LiteDB;


namespace DiscordBot.Modules

{
    public static class ViolationManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="violationType"> Violation type. 1 = Ban, 2 = Kick, 3 = mute, 4 = warn </param>
        /// <param name="violator">User that committed the violation</param>
        /// <param name="reason">Reason for the violation</param>
        /// <param name="context">Command Context</param>
        /// <returns>Embed</returns>
        public static Embed NewViolation(SocketGuildUser violator, string reason, SocketCommandContext context, int violationType = 0)
        {
            try
            {
                DateTime date = DateTime.Now;

//                string violationQuery =
//                    "INSERT INTO Violations (UserID, ViolationDate, ViolationType, Reason) VALUES (@0, @1, @2, @3)";
//                SqlHandler.NewViolationQuery(violationQuery, user.Id, date, violationType, reason);
//
//                string violationIDQuery = "SELECT ViolationID FROM Violations WHERE ViolationDate = @0";
//                List<object> result = SqlHandler.SelectQuery(violationIDQuery, date);
//                string violationId = result[0].ToString();

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
                
                Embed violationEmbed = ViolationEmbed(violation, context);
                return violationEmbed;
            }
            catch (IndexOutOfRangeException)
            {
                Embed error = new EmbedBuilder
                    {
                        Title = "Te weinig argumenten"
                    }
                    .WithDescription("Commando is uitgevoerd met te weinig argumenten")
                    .AddField("Example", DiscordBot.Config["CommandPrefix"]+"ban [user] {reason}")
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

//        public static string ViolationCount(ulong userId)
//        {
//            string getCountQuery = "SELECT COUNT(ViolationID) FROM Violations WHERE UserID = @0";
//            List<object> result = SqlHandler.SelectQuery(getCountQuery, userId);
//            string violationCount = result[0].ToString();
//            return violationCount;
//        }

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
                .AddField("User:", "<@!"+ violation.UserId+">", true)
                .AddField("Date:", DateTime.Now, true)
                .AddField("Moderator:", context.User.Mention)
                .AddField("Reason:", violation.Reason)
                .AddField("Violation ID:", violation.Id, true)
                .WithCurrentTimestamp()
                .WithFooter("UserID: " + violation.UserId)
                .Build();

            return embed;
        }
        
        public static void InsertViolation(Violation record) {
            using (var db = new LiteDatabase(DiscordBot.Config + "Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                table.Insert(record);
            }
        }

//        public static void DeleteViolationRecord() {
//            using (var db = new LiteDatabase(DiscordBot.Config + "Database.db"))
//            {
//                var table = db.GetCollection<Violation>("violations");
//
//                table.Delete();
//            }
//        }
//        
        public static Violation GetCreatedRecord(DateTime date) {
            using (var db = new LiteDatabase(DiscordBot.Config + "Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                return table.FindOne(x => x.Date == date);
            }
        }
        
        public static Violation GetViolationRecord(int id) {
            using (var db = new LiteDatabase(DiscordBot.Config + "Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");

                return table.FindOne(x => x.Id == id);
            }
        }
        
        
    }
}