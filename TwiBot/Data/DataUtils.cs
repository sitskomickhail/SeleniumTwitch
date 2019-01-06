using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwiBot.Data
{
    internal class DataUtils
    {
        public static MySqlConnection GetDBConnection()
        {
            string host = "141.8.192.151";
            int port = 3306;
            string database = "******";
            string username = "******";
            string password = "******";

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
