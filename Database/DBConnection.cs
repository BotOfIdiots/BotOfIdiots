using System;
using System.Xml;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public class DbConnection
    {
        #region Fields
        public readonly MySqlConnection SqlConnection;
        #endregion
        
        #region Constructors
        public DbConnection(XmlNodeList settings)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.SslMode = MySqlSslMode.None;
            builder.CharacterSet = "utf8mb4";
            builder.AllowUserVariables = true;

            foreach (XmlNode node in settings)
            {
                switch (node.Name)
                {
                    case "User":
                        builder.UserID = node.InnerText;
                        break;
                    case "Password":
                        builder.Password = node.InnerText;
                        break;
                    case "Server":
                        builder.Server = node.InnerText;
                        break;
                    case "Database":
                        builder.Database = node.InnerText;
                        break;
                    case "Port":
                        builder.Port = Convert.ToUInt32(node.InnerText);
                        break;
                }
            }

            SqlConnection = new MySqlConnection(builder.ConnectionString);
            CheckConnection();
        }
        #endregion
        
        #region Methods
        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            CheckConnection();
            try
            {
                using (SqlConnection)
                {
                    using (MySqlCommand cmd = SqlConnection.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.Parameters.AddRange(parameters);

                        int returnVal = cmd.ExecuteNonQuery();
                        return returnVal;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SqlConnection.Close();
                return -1;
            }
        }


        public void CheckConnection()
        {
            if (!SqlConnection.Ping())
            {
                try
                {
                    SqlConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        #endregion
    }
}