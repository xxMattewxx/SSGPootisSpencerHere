using MySqlConnector;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SSGDistributer.Handlers
{
    class GetHash
    {
        public static string GetTaskChecksum(int taskID)
        {
            try
            {
                var connection_string = Global.builder.ConnectionString;
                using DbConnection conn = Global.dbtype switch
                {
                    "PSQL" => new NpgsqlConnection(connection_string),
                    "MYSQL" => new MySqlConnection(connection_string),
                    _ => throw new Exception("Invalid db type " + Global.dbtype),
                };
                conn.Open();

                using var command = conn.CreateCommand();

                command.CommandText = "SELECT MD5(task_str) FROM tasks WHERE task_id = @taskID LIMIT 1;";

                DbParameter taskIdParam = command.CreateParameter();
                taskIdParam.ParameterName = "@taskID";
                taskIdParam.Value = taskID;
                taskIdParam.DbType = System.Data.DbType.Int32;
                command.Parameters.Add(taskIdParam);

                using DbDataReader reader = command.ExecuteReader();
                reader.Read();

                if (!reader.HasRows)
                    return null;

                return reader.GetString(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            NameValueCollection query = context.Request.QueryString;
            string taskStr = query.Get("taskid");
            int taskID;

            if (!Int32.TryParse(taskStr, out taskID) || taskID < 0)
            {
                writer.WriteLine("Invalid task ID!");
                return;
            }

            string checksum = GetTaskChecksum(taskID);
            if (checksum == null)
            {
                writer.WriteLine("Could not retrieve task!");
                return;
            }

            writer.Write(checksum);
            return;
        }
    }
}
