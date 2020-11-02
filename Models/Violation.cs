using System;

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
        public DateTime Expires { get; set; }
    }
}