using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;


namespace DiscordBot.Modules

{
    public static class ViolationManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="violationType"> Violation type. 1 = Ban, 2 = Kick, 3 = mute, 4 = warn </param>
        /// <param name="user">User that committed the violation</param>
        /// <param name="reason">Reason for the violation</param>
        /// <param name="context">Command Context</param>
        /// <returns>Embed</returns>
        public static Embed NewViolation(IUser user, string reason, SocketCommandContext context, string violationType = "0")
        {
            try
            {
                string date = context.Message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

                string violationQuery =
                    "INSERT INTO Violations (UserID, ViolationDate, ViolationType, Reason) VALUES (@0, @1, @2, @3)";
                SqlHandler.Query(violationQuery, user.Id.ToString(), date, violationType, reason);

                string violationIDQuery = "SELECT ViolationID FROM Violations WHERE ViolationDate = @0";
                List<object> result = SqlHandler.SelectQuery(violationIDQuery, date);
                string violationId = result[0].ToString();

                Embed violationEmbed = ViolationEmbed(user, reason, violationId, context, violationType);
                return violationEmbed;
            }
            catch (NullReferenceException e)
            {
                Embed error = new EmbedBuilder
                {
                    Title = "NullReferenceException"
                }
                    .WithDescription(e.ToString())
                    .AddField("Time", DateTime.Now)
                    .Build();

                return error;
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
                    .WithDescription(e.ToString())
                    .AddField("Time", DateTime.Now)
                    .Build();

                return error;
            }
        }

        public static string ViolationCount(string userId)
        {
            string getCountQuery = "SELECT COUNT(ViolationID) FROM Violations WHERE UserID = @0";
            List<object> result = SqlHandler.SelectQuery(getCountQuery, userId);
            string violationCount = result[0].ToString();
            return violationCount;
        }

        // private static Embed GetViolation(int violationID)
        // {
        //     return Embed(user, reason, violationID, context, violationType);
        // }

        private static Embed ViolationEmbed(IUser user, string reason, string violationId, SocketCommandContext context, string violationType)
        {
            String violationTitle;
            switch (violationType)
            {
                case "1":
                    violationTitle = "Banned";
                    break;
                case "2":
                    violationTitle = "Kicked";
                    break;
                case "3":
                    violationTitle = "Muted";
                    break;
                case "4":
                    violationTitle = "Unmuted";
                    break;
                case "5":
                    violationTitle = "Unbanned";
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
                .AddField("User:", user.Mention, true)
                .AddField("Date:", DateTime.Now, true)
                .AddField("Moderator:", context.User.Mention)
                .AddField("Reason:", reason)
                .AddField("Violation ID:", violationId, true)
                .WithCurrentTimestamp()
                .WithFooter("UserID: " + user.Id)
                .Build();

            return embed;
        }
    }
}