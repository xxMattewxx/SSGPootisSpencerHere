using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Data.Common;
using System.IO;

namespace SSGDistributer
{
    class Global
    {
        public static DbConnectionStringBuilder builder;
        public static String dbtype;

        public static void Load()
        {
            //builder = JsonConvert.DeserializeObject<MySqlConnectionStringBuilder>(
            //    File.ReadAllText("sqlconnection.cfg")
            //);
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            dbtype = Environment.GetEnvironmentVariable("DB_TYPE");
            if (dbtype == null) throw new Exception("No DB_TYPE");
            builder = dbtype switch {
                "MYSQL" => new MySqlConnectionStringBuilder() {
                    // The Cloud SQL proxy provides encryption between the proxy and instance.
                    SslMode = MySqlSslMode.None,
                    // Remember - storing secrets in plain text is potentially unsafe. 
                    Server = Environment.GetEnvironmentVariable("DB_HOST"),
                    UserID = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user
                    Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                    Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'
                    Pooling = true,
                    AllowUserVariables = true,
                },
                "PSQL" => new NpgsqlConnectionStringBuilder() {
                    // Remember - storing secrets in plain text is potentially unsafe. 
                    Host = Environment.GetEnvironmentVariable("DB_HOST"),
                    Username = Environment.GetEnvironmentVariable("DB_USER"),   // e.g. 'my-db-user
                    Password = Environment.GetEnvironmentVariable("DB_PASS"), // e.g. 'my-db-password'
                    Database = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'

                },
                _ => throw new Exception("Invalid DB_TYPE : " + dbtype),
            };
            Console.WriteLine(builder.ConnectionString);
        }
    }
}