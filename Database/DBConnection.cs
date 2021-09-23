using System;
using System.Xml;
using MySql.Data.MySqlClient;

namespace DiscordBot.Database
{
    public class DbConnection
    {
        private MySqlConnection _sqlConnection;

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

            _sqlConnection = new MySqlConnection(builder.ConnectionString);
            CheckConnection();
        }

        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            CheckConnection();
            try
            {
                using (_sqlConnection)
                {
                    using (MySqlCommand cmd = _sqlConnection.CreateCommand())
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
                _sqlConnection.Close();
                return -1;
            }
        }

        public MySqlDataReader ExecuteReader(string query, params MySqlParameter[] parameters)
        {
            CheckConnection();

            try
            {
                using (_sqlConnection)
                {
                    using (MySqlCommand cmd = _sqlConnection.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.Parameters.AddRange(parameters);
                        
                        return cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _sqlConnection.Close();
                return null;
            }
        }

        private void CheckConnection()
        {
            if (!_sqlConnection.Ping())
            {
                try
                {
                    _sqlConnection.Open();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}