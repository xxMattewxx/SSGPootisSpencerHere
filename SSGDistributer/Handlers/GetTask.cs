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
    class GetTask
    {
        public static string GetTaskInfo(int taskID)
        {
            try
            {
                var connection_string = Global.builder.ConnectionString;
                using DbConnection conn = Global.dbtype switch {
                    "PSQL" => new NpgsqlConnection(connection_string),
                    "MYSQL" => new MySqlConnection(connection_string),
                    _ => throw new Exception("Invalid db type " + Global.dbtype),
                };
                conn.Open();

                using var command = conn.CreateCommand();
                
                command.CommandText = Global.dbtype switch {
                    "PSQL" => "SELECT structure_seed,string_agg(CONCAT(mc_version, ' ', chunk_x,' ',chunk_z), ' ' ORDER BY id) " +
                    "FROM seeds " +
                    "WHERE task_id = @taskID " +
                    "GROUP BY structure_seed " +
                    "LIMIT 300;",
                    "MYSQL" => "SELECT structure_seed,GROUP_CONCAT(CONCAT(mc_version, ' ', chunk_x,' ',chunk_z) ORDER BY id ASC SEPARATOR ' ') " +
                    "FROM seeds " +
                    "WHERE task_id = @taskID " +
                    "GROUP BY structure_seed " +
                    "LIMIT 300;",
                    _ => throw new Exception("Invalid db type " + Global.dbtype),
                }; 
                    
                DbParameter taskIdParam = command.CreateParameter();
                taskIdParam.ParameterName = "@taskID";
                taskIdParam.Value = taskID;
                taskIdParam.DbType = System.Data.DbType.Int32;
                command.Parameters.Add(taskIdParam);

                using DbDataReader reader = command.ExecuteReader();
                reader.Read();

                if (!reader.HasRows)
                    return null;

                string ret = "";
                do
                {
                    ret += reader.GetInt64(0) + " " + reader.GetString(1) + "\n";
                }
                while (reader.Read());

                return ret;
            }
            catch(Exception e)
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

            if(!Int32.TryParse(taskStr, out taskID) || taskID < 0)
            {
                writer.WriteLine("Invalid task ID!");
                return;
            }

            string task = GetTaskInfo(taskID);
            if(task == null)
            {
                writer.WriteLine("Could not retrieve task!");
                return;
            }

            writer.Write(task);
            return;
        }
    }
}
