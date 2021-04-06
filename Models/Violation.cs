using System;
using Discord.WebSocket;
using LiteDB;

namespace DiscordBot.Models
{
    /// <summary>
    /// This object Contains all the information for a violation
    /// </summary>
    public class Violation
    {
        public int Id { get; set; }
        public ulong UserId { get; set; }
        public ulong ModeratorId { get; set; }
        public int Type { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public DateTime Expires { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Violation()
        {
        }

        /// <summary>
        /// Creates a new violation
        /// </summary>
        /// <param name="violater">The user of the violation</param>
        /// <param name="moderatorId">The moderator of the violation</param>
        /// <param name="type">The type of the violation</param>
        /// <param name="reason">The reason for the violation</param>
        public Violation(ulong violater, ulong moderatorId, int type, string reason)
        {
            SetValues(violater, moderatorId, type, reason);
        }

        /// <summary>
        /// Creates a new violation
        /// </summary>
        /// <param name="violater">The user of the violation</param>
        /// <param name="moderatorId">The moderator of the violation</param>
        /// <param name="type">The type of the violation</param>
        /// <param name="reason">The reason for the violation</param>
        /// <param name="expires">The moment the violation expirers</param>
        public Violation(ulong violater, ulong moderatorId, int type, string reason, DateTime expires)
        {
            SetValues(violater, moderatorId, type, reason);
            Expires = expires;
        }

        /// <summary>
        /// Sets the basic violation information
        /// </summary>
        /// <param name="userId">The user of the violation</param>
        /// <param name="moderatorId">The moderator of the violation</param>
        /// <param name="type">The type of the violation</param>
        /// <param name="reason">The reason for the violation</param>
        private void SetValues(ulong userId, ulong moderatorId, int type, string reason)
        {
            UserId = userId;
            ModeratorId = moderatorId;
            Type = type;
            Reason = reason;
            Date = DateTime.Now;
        }

        /// <summary>
        /// Create a new record and return it's id
        /// </summary>
        /// <returns>int</returns>
        public int InsertRecord()
        {
            using (var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db"))
            {
                var table = db.GetCollection<Violation>("violations");
                table.Insert(this);
            }
            return CreatedRecord(Date);
        }

        /// <summary>
        /// Return the id of record for the given date and time
        /// </summary>
        /// <param name="dateTime">The date and time of the record to return </param>
        /// <returns>int</returns>
        private static int CreatedRecord(DateTime dateTime)
        {
            using var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db");
            var table = db.GetCollection<Violation>("violations");

            return table.FindOne(x => x.Date == dateTime).Id;
        }

        /// <summary>
        /// Returns a violation
        /// </summary>
        /// <param name="violationId">The id of the violation to return</param>
        /// <returns></returns>
        public static Violation GetRecord(int violationId)
        {
            using var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db");
            var table = db.GetCollection<Violation>("violations");
                
            return table.FindOne(x => x.Id == violationId);
        }

        /// <summary>
        /// Deletes a violation
        /// </summary>
        /// <param name="violationId">The id of the violation to delete</param>
        public static void DeleteRecord(int violationId)
        {
            using var db = new LiteDatabase(DiscordBot.WorkingDirectory + "/Database.db");
            var table = db.GetCollection<Violation>("violations");

            table.Delete(violationId);
        }
    }
}