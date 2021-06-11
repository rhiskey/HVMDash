using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HVMDash.Server
{
    public class Program
    {
        public static string SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET, MySQLConnection, MSSQLConnection;
        public static void Main(string[] args)
        {
            DotNetEnv.Env.TraversePath().Load();
            SPOTIFY_CLIENT_ID = DotNetEnv.Env.GetString("SPOTIFY_CLIENT_ID");
            SPOTIFY_CLIENT_SECRET = DotNetEnv.Env.GetString("SPOTIFY_CLIENT_SECRET");
            string server = DotNetEnv.Env.GetString("MYSQL_SERVER");
            string user = DotNetEnv.Env.GetString("MYSQL_USER");
            string password = DotNetEnv.Env.GetString("MYSQL_PASS");
            string database = DotNetEnv.Env.GetString("MYSQL_DB");
            int port = DotNetEnv.Env.GetInt("MYSQL_PORT");
            string mysqlConn = $"server={server};port={port};user={user};password={password};database={database}";

            MySQLConnection = mysqlConn;

            //string mssqlServer = DotNetEnv.Env.GetString("MSSQL_SERVER");
            //MySQLConnection = mysqlConn;
            //MSSQLConnection = $"Server={mssqlServer};Database={};User Id={};Password={};MultipleActiveResultSets=true;";

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
