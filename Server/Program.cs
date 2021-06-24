using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace HVMDash.Server
{
    public class Program
    {
        public static string SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET;
        public static void Main(string[] args)
        {
            vkaudioposter_ef.Model.Configuration cfg;

            using (var context = new vkaudioposter_ef.AppContext())
            {
                cfg = context.Configurations.FirstOrDefault();
            }
            SPOTIFY_CLIENT_ID = cfg.SpotifyClientId;
            SPOTIFY_CLIENT_SECRET = cfg.SpotifyClientSecret;


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
