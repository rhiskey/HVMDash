using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HVMDash.Server
{
    public class Program
    {
        public static string SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET, MSSQLConnection;
        public static void Main(string[] args)
        {
            DotNetEnv.Env.TraversePath().Load();
            SPOTIFY_CLIENT_ID = DotNetEnv.Env.GetString("SPOTIFY_CLIENT_ID");
            SPOTIFY_CLIENT_SECRET = DotNetEnv.Env.GetString("SPOTIFY_CLIENT_SECRET");

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
