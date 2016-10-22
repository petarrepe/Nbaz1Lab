using Npgsql;
using System.Text;

namespace Nbaz1Lab
{
    public static class DatabaseHelper
    {
        private static string connString = string.Format("Server=192.168.56.12;Port=5432;" +
                       "User Id=postgres;Password=reverse;Database=postgres;");
        private static NpgsqlConnection conn;

        public static bool Insert(string tableName, params string[] arguments)
        {
            conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sb = new StringBuilder("INSERT INTO \"" + tableName+"\" (body, summary, keywords, title) VALUES (");
            for (int i=0; i < arguments.Length; i++)
            {
                sb.Append("'").Append(arguments[i]).Append("',");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");

            NpgsqlCommand cmd = new NpgsqlCommand(sb.ToString(), conn);
            int affectedRows = cmd.ExecuteNonQuery();

            conn.Close();

            return affectedRows == 0 ? false : true;
        }


    }
}
