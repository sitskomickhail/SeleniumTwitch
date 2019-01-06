using MySql.Data.MySqlClient;
using System;
using TwiBot.Data.UIDATA;

namespace TwiBot.Data
{
    public static class DBWorker
    {
        private static MySqlConnection conn;

        static DBWorker()
        {
            conn = DataUtils.GetDBConnection();
        }

        public static bool SetLicenseKey(string lKey)
        {
            conn.Open();
            string sql = "UPDATE LicenseTBO SET active_key = @active1 WHERE license_key = @license AND active_key = @active2";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@active1", MySqlDbType.Int32).Value = 1;
                cmd.Parameters.Add("@active2", MySqlDbType.Int32).Value = 0;
                cmd.Parameters.Add("@license", MySqlDbType.String).Value = lKey;
                try
                {
                    if (cmd.ExecuteNonQuery() != 1) { conn.Close(); conn.Dispose(); return false; };
                    conn.Close();
                    conn.Dispose();
                    return true;
                }
                catch
                {
                    conn.Close();
                    conn.Dispose();
                    return false;
                }
            }
        }

        public static void INSERTDATETIME()
        {
            conn.Open();
            string sql = "INSERT INTO LicenseTBO (license_key, duration_key, active_key) VALUES (@lic, @date, @act)";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.Add("@lic", MySqlDbType.String).Value = "TH1272";
            cmd.Parameters.Add("@date", MySqlDbType.DateTime).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            cmd.Parameters.Add("@act", MySqlDbType.Int32).Value = 0;

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static bool IsLicenseKey_Exist()
        {
            string lcKey = Crypting.DeCryptData(); if (lcKey == null) return false;
            lcKey = lcKey.Replace("\r\n", "");
            conn.Open();
            string sql = "SELECT * from LicenseTBO WHERE license_key = @license AND duration_key > @curdate"; //todo: date check
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.Add("@license", MySqlDbType.String).Value = lcKey;
            cmd.Parameters.Add("@curdate", MySqlDbType.DateTime).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            try
            {
                if (cmd.ExecuteNonQuery() != 1) { conn.Close(); conn.Dispose(); return false; }
                conn.Close();
                conn.Dispose();
                return true;
            }
            catch
            {
                conn.Close();
                conn.Dispose();
                return false;
            }
        }
    }
}