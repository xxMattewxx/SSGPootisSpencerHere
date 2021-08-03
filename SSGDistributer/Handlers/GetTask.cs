using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                using var conn = new MySqlConnection(Global.builder.ConnectionString);
                conn.Open();

                using var command = conn.CreateCommand();

                command.CommandText = "" +
                    "SELECT structure_seed,GROUP_CONCAT(CONCAT(mc_version, ' ', chunk_x,' ',chunk_z) SEPARATOR ' ') " +
                    "FROM seeds " +
                    "WHERE task_id = @taskID " +
                    "GROUP BY structure_seed " +
                    "LIMIT 300;";

                command.Parameters.AddWithValue("@taskID", taskID);

                MySqlDataReader reader = command.ExecuteReader();
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
