using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using DiscordBot.Models.Embeds;
using DiscordBot.Modules.Commands;
using LiteDB;

namespace DiscordBot

{
    public static class ViolationManager
    {
        /// <summary>
        /// Create a New violation insert it into the database and return an embed
        /// </summary>
        /// <param name="violationType"> Violation type. 0 = warn, 1 = Ban, 2 = Kick, 3 = mute, 4 = unmute</param>
        /// <param name="violator">User that committed the violation</param>
        /// <param name="reason">Reason for the violation</param>
        /// <param name="context">Command Context</param>
        /// <returns>Embed</returns>
        public static async Task<Embed> NewViolation(SocketGuildUser violator, string reason, SocketCommandContext context,
            ViolationTypes violationType = ViolationTypes.Warned)
        {
            Embed violationEmbed = ExecuteViolation(violator, reason, context, violationType);
            
            await Functions.SendMessageEmbedToUser(violator, violationEmbed, context);

            return violationEmbed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="violator"></param>
        /// <param name="reason"></param>
        /// <param name="context"></param>
        /// <param name="violationType"></param>
        /// <returns></returns>
        private static Embed ExecuteViolation(SocketGuildUser violator, string reason, SocketCommandContext context,
            ViolationTypes violationType)
        {
            switch (violationType)
            {
                case ViolationTypes.Banned:
                    violator.BanAsync(1, reason);
                    break;
                case ViolationTypes.Kicked:
                    violator.KickAsync(reason);
                    break;
                case ViolationTypes.Muted:
                    violator.AddRoleAsync(Punishment.MutedRole);
                    break;
                case ViolationTypes.UnMuted:
                    violator.RemoveRoleAsync(Punishment.MutedRole);
                    break;
            }
            
            return CreateViolationRecord(violator.Id, reason, context, violationType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="violator"></param>
        /// <param name="reason"></param>
        /// <param name="context"></param>
        /// <param name="violationType"></param>
        /// <returns></returns>
        public static Embed CreateViolationRecord(ulong violator, string reason, SocketCommandContext context,
            ViolationTypes violationType)
        {
            int violationId = new Violation(violator, context.User.Id, violationType, reason)
                .InsertRecord();
            
            return new ViolationEmbedBuilder(violationId, context.Client.CurrentUser).Build();
        }

        /// <summary>
        /// Returns the total ammount of violations for a specified user
        /// </summary>
        /// <param name="userId">id of the user to return the violation count of</param>
        /// <returns>int</returns>
        public static int CountUserViolations(ulong userId)
        {
            using var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db");
            var table = db.GetCollection<Violation>("violations");

            return table.Count(x => x.UserId == userId);
        }

        /// <summary>
        /// Returns a list of violations commited by the specified user
        /// </summary>
        /// <param name="user">The user of which to return the violations</param>
        /// <returns>List<Violation></returns>
        public static List<Violation> GetViolations(ulong user)
        {
            using var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db");
            
            var table = db.GetCollection<Violation>("violations");
            
            IEnumerable<Violation> queryData = table.Find(x => x.UserId == user);
            
            return CreateViolationList(queryData);
        }

        /// <summary>
        /// Creates a List of Violations from the given IEnumerable
        /// </summary>
        /// <param name="violations"></param>
        /// <returns></returns>
        private static List<Violation> CreateViolationList(IEnumerable<Violation> violations)
        {
            return violations.ToList();
        }

    }
}