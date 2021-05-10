using System;
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
        public ViolationTypes Type { get; set; }
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
        public Violation(ulong violater, ulong moderatorId, ViolationTypes type, string reason)
        {
            UserId = violater;
            ModeratorId = moderatorId;
            Type = type;
            Reason = reason;
            Date = DateTime.Now;
        }

        /// <summary>
        /// Creates a new violation
        /// </summary>
        /// <param name="violater">The user of the violation</param>
        /// <param name="moderatorId">The moderator of the violation</param>
        /// <param name="type">The type of the violation</param>
        /// <param name="reason">The reason for the violation</param>
        /// <param name="expires">The moment the violation expirers</param>
        public Violation(ulong violater, ulong moderatorId, ViolationTypes type, string reason, DateTime expires)
            : this(violater, moderatorId, type, reason)
        {
            Expires = expires;
        }

        /// <summary>
        /// Create a new record and return it's id
        /// </summary>
        /// <returns>int</returns>
        public int InsertRecord()
        {
            var table = DiscordBot.BotService.Database.GetCollection<Violation>("violations");
            table.Insert(this);
            
            return GetRecordByDate(Date);
        }

        /// <summary>
        /// Return the id of record for the given date and time
        /// </summary>
        /// <param name="dateTime">The date and time of the record to return </param>
        /// <returns>int</returns>
        private static int GetRecordByDate(DateTime dateTime)
        {
            var table = DiscordBot.BotService.Database.GetCollection<Violation>("violations");
            
            return table.FindOne(x => x.Date == dateTime).Id;
        }

        /// <summary>
        /// Returns a violation
        /// </summary>
        /// <param name="violationId">The id of the violation to return</param>
        /// <returns></returns>
        public static Violation GetRecordById(int violationId)
        {
            var table = DiscordBot.BotService.Database.GetCollection<Violation>("violations");
            
            return table.FindOne(x => x.Id == violationId);
        }

        /// <summary>
        /// Deletes a violation
        /// </summary>
        /// <param name="violationId">The id of the violation to delete</param>
        public static void DeleteRecord(int violationId)
        {
            var table = DiscordBot.BotService.Database.GetCollection<Violation>("violations");
            
            table.Delete(violationId);
        }
    }
}