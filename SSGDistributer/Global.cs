using MySqlConnector;
using Newtonsoft.Json;
using System.IO;

namespace SSGDistributer
{
    class Global
    {
        public static MySqlConnectionStringBuilder builder;

        public static void Load()
        {
            builder = JsonConvert.DeserializeObject<MySqlConnectionStringBuilder>(
                File.ReadAllText("sqlconnection.cfg")
            );
        }
    }
}