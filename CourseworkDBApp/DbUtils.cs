using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace CourseworkDBApp
{
    public static partial class DbUtils
    {
        public static string connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=shi";

        public static DataTable GetDataTable(string sql, params NpgsqlParameter[] parameters)
        {
            using var conn = new NpgsqlConnection(connStr);
            using var cmd = new NpgsqlCommand(sql, conn);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using var da = new NpgsqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        public static NpgsqlDataAdapter GetDataAdapter(string sql)
        {
            var conn = new NpgsqlConnection(connStr);
            var da = new NpgsqlDataAdapter(sql, conn);
            var cb = new NpgsqlCommandBuilder(da);
            return da;
        }
        public static List<string> GetTableNames()
        {
            var names = new List<string>();
            var sql = @"SELECT table_name 
                        FROM information_schema.tables
                        WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
                        ORDER BY table_name;";
            using var conn = new NpgsqlConnection(DbUtils.connStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                names.Add(rdr.GetString(0));
            return names;
        }

        public static List<string> GetViewNames()
        {
            var names = new List<string>();
            var sql = @"SELECT table_name 
                        FROM information_schema.tables
                        WHERE table_schema = 'public' AND table_type = 'VIEW'
                        ORDER BY table_name;";
            using var conn = new NpgsqlConnection(DbUtils.connStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                names.Add(rdr.GetString(0));
            return names;
        }

        public static List<(string name, string dataType, bool isNullable)> GetColumns(string table)
        {
            var col = new List<(string, string, bool)>();
            var sql = @"SELECT column_name, data_type, is_nullable
                        FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = @table
                        ORDER BY ordinal_position;";
            using var conn = new NpgsqlConnection(DbUtils.connStr);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("table", table);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var name = rdr.GetString(0);
                var dtype = rdr.GetString(1);
                var isNullable = rdr.GetString(2) == "YES";
                col.Add((name, dtype, isNullable));
            }
            return col;
        }
    }
}
