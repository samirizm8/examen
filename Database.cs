using MySql.Data.MySqlClient;

namespace GestionExamensApp
{
    public static class Database
    {
        private static string connStr = "server=localhost;database=gestion_examens;user=root;password=;";
        public static MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            return conn;
        }
    }
}
