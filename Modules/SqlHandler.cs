using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;

namespace DiscordBot.Modules
{
    public static class SqlHandler
    {
        private static readonly string ConnectionString = "Server=" + DiscordBot.Config["DbHost"] + "; Database=" 
            + DiscordBot.Config["DbSchema"] + "; Uid=" + DiscordBot.Config["DbUsername"] + "; Pwd=" + DiscordBot.Config["DbPassword"] + ";";
        private static MySqlConnection _cnn;
        
        public static List<object> SelectQuery(string query, params String[] queryParameters)
        {
            List<object> result = new List<object>();
            
            using (_cnn = new MySqlConnection(ConnectionString))
            {
                _cnn.Open();
                MySqlCommand command = SqlStatement(query, queryParameters);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetValue(0));
                    }
                }
            }

            return result;
        }

        private static MySqlCommand SqlStatement(string query, String[] parameters)
        {
            MySqlCommand command = new MySqlCommand(query, _cnn);
            
            for(int pos = 0; pos < parameters.Length; pos++)
            {
                command.Parameters.Add(new MySqlParameter(pos.ToString(), parameters[pos]));
            }
            
            return command;
        }

        public static void Query(string query, params String[] queryParameters)
        {
            try
            {
                using (_cnn = new MySqlConnection(ConnectionString))
                {
                    _cnn.Open();
                    MySqlCommand command = SqlStatement(query, queryParameters);
                    command.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


    }
}