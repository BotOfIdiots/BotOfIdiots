using System;
using System.Data;
using DiscordBot.Database;
using DiscordBot.Modules;
using MySql.Data.MySqlClient;

namespace DiscordBot.Models
{
    /// <summary>
    /// This object contains all the information for a violation
    /// </summary>
    public class Violation
    {
        #region Fields

        public ulong Guild { get; set; }
        public int ViolationId { get; set; }
        public ulong User { get; set; }
        public ulong Moderator { get; set; }
        public bool Confidential { get; set; }
        public int Type { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public DateTime Expires { get; set; }

        #endregion

        #region Constructors

        public Violation(ulong guild)
        {
            Guild = guild;
            GenerateViolationId();
        }

        public Violation(ulong guild, ulong userId, ulong moderatorId, int type, string reason,
            DateTime date, int violationId = -1, bool confidential = false)
        {
            Guild = guild;
            User = userId;
            Moderator = moderatorId;
            Confidential = confidential;
            Type = type;
            Reason = reason;
            Date = date;

            if (violationId != -1)
            {
                ViolationId = violationId;
            }
            else
            {
                GenerateViolationId();
            }
        }

        public Violation(ulong guild, ulong userId, ulong moderatorId, int type, string reason,
            DateTime date, DateTime expires, int violationId = -1, bool confidential = false) :
            this(guild, userId, moderatorId, type, reason, date, violationId, confidential)
        {
            Expires = expires;
        }

        #endregion

        #region Methods

        private void GenerateViolationId()
        {
            int id = 0;
            string query = "SELECT max(ViolationId) as maxId FROM violations WHERE Guild = @Guild";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64)
            {
                Value = Guild
            };

            #endregion

            try
            {
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild);

                while (reader.Read())
                {
                    if (reader.HasRows && reader.VisibleFieldCount > 0)
                    {
                        id = reader.GetInt32("maxId") + 1;
                    }
                }
            }

            #region Exception Handling

            catch (MySqlException ex)
            {
                // if (ex.Message.Contains("Data is Null")) return 0;
                EventHandlers.LogException(ex, DiscordBot.Client.GetGuild(Guild));
            }

            #endregion

            finally
            {
                ViolationId = id;
            }
        }

        #endregion

        #region Database Operations

        public void Insert()
        {
            string query =
                "INSERT INTO violations VALUE (@Guild, @ViolationId, @User, @Moderator, @Confidential, @Type, @Reason, @Date, @Expires)";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64);
            guild.Value = Guild;

            MySqlParameter violationId = new MySqlParameter("@ViolationId", MySqlDbType.Int32);
            violationId.Value = ViolationId;

            MySqlParameter user = new MySqlParameter("@User", MySqlDbType.UInt64);
            user.Value = User;


            MySqlParameter moderator = new MySqlParameter("@Moderator", MySqlDbType.UInt64);
            moderator.Value = Moderator;

            MySqlParameter confidential = new MySqlParameter("@Confidential", MySqlDbType.Int16);
            confidential.Value = Confidential;

            MySqlParameter type = new MySqlParameter("@Type", MySqlDbType.Int16);
            type.Value = Type;

            MySqlParameter reason = new MySqlParameter("@Reason", MySqlDbType.VarChar);
            reason.Value = Reason;

            MySqlParameter date = new MySqlParameter("@Date", MySqlDbType.DateTime);
            date.Value = Date;

            MySqlParameter expires = new MySqlParameter("@Expires", MySqlDbType.DateTime);
            expires.Value = Expires;

            #endregion

            try
            {
                DiscordBot.DbConnection.ExecuteNonQuery(query, guild, violationId, user, moderator, confidential, type,
                    reason, date, expires);
            }

            #region Exception Handling

            catch (MySqlException ex)
            {
            }

            catch (Exception ex)
            {
                EventHandlers.LogException(ex, DiscordBot.Client.GetGuild(Guild));
            }

            #endregion
        }

        public static Violation Select(ulong guildId, int violationId)
        {
            string query =
                "SELECT User, Moderator, Confidential, Type, Reason, Date, Expires FROM violations WHERE Guild = @Guild AND ViolationId = @ViolationId";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = guildId };

            MySqlParameter violation = new MySqlParameter("@ViolationId", MySqlDbType.Int32) { Value = violationId };

            #endregion

            try
            {
                DiscordBot.DbConnection.CheckConnection();
                using MySqlConnection conn = DiscordBot.DbConnection.SqlConnection;
                MySqlDataReader reader = DbOperations.ExecuteReader(conn, query, guild, violation);

                while (reader.Read())
                {
                    ulong user = reader.GetUInt64("User");
                    ulong moderator = reader.GetUInt64("Moderator");
                    bool confidential = reader.GetBoolean("Confidential");
                    int type = reader.GetInt32("Type");
                    string reason = reader.GetString("Reason");
                    DateTime date = reader.GetDateTime("Date");
                    DateTime expires = reader.GetDateTime("Expires");

                    return new Violation(guildId, user, moderator, type, reason, date, expires,
                        violationId, confidential);
                }
            }

            #region Exception Handling

            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
            }

            catch (Exception ex)
            {
                EventHandlers.LogException(ex, DiscordBot.Client.GetGuild(guildId));
            }

            #endregion

            return null;
        }

        public void Remove()
        {
            string query = "DELETE FROM violations WHERE Guild = @Guild AND ViolationId = @ViolationId";

            #region SQL Parameters

            MySqlParameter guild = new MySqlParameter("@Guild", MySqlDbType.UInt64) { Value = Guild };

            MySqlParameter violation = new MySqlParameter("@ViolationId", MySqlDbType.Int32) { Value = ViolationId };

            #endregion

            try
            {
                DiscordBot.DbConnection.ExecuteNonQuery(query, guild, violation);
            }

            #region Exception Handling

            catch (MySqlException ex)
            {
            }

            catch (Exception ex)
            {
                EventHandlers.LogException(ex, DiscordBot.Client.GetGuild(Guild));
            }

            #endregion
        }

        #endregion
    }
}