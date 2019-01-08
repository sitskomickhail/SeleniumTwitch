using MySql.Data.MySqlClient;
using System;

namespace TwiBot.Data
{
    internal class DataUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "141.8.192.151";
            int port = 3306;
            string database = "f0265686_TwitchBots_dataBase";
            string username = "f0265686";
            string password = "arnefiixtu";

            return GetDBConnection(host, port, database, username, password);
        }

        private static MySqlConnection GetDBConnection(string host, int port, string database, string username, string password)
        {
            String connString = "Server=" + host + ";Database=" + database
                + ";port=" + port + ";User Id=" + username + ";password=" + password;

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }
    }
}
