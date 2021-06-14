using HVMDash.Server.Context;
using HVMDash.Server.Hubs;
using HVMDash.Server.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace HVMDash.Server
{
    public class Startup
    {
        //private static string mySQLConn;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            //MSSQL Configuration.GetConnectionString("MSSQL")
            //services.AddDbContext<PlaylistContext>(opt => opt.UseSqlServer(Program.MSSQLConnection));
            //services.AddDbContext<ConsolePhotostockContext>(opt => opt.UseSqlServer(Program.MSSQLConnection));
            //services.AddDbContext<PostedTracksContext>(opt => opt.UseSqlServer(Program.MSSQLConnection));
            //services.AddDbContext<ParserXpathContext>(opt => opt.UseSqlServer(Program.MSSQLConnection));
            //services.AddDbContext<PostContext>(opt => opt.UseSqlServer(Program.MSSQLConnection));

            services.AddDbContext<PlaylistContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<ConsolePhotostockContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<PostedTracksContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<ParserXpathContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<PostContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));

            services.AddMemoryCache();
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            ); //N+1
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddHostedService<TimedHostedService>();

            //services.AddHostedService<ConsumeScopedServiceHostedService>();
            //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<CommandsHub>("/commandshub");
                endpoints.MapFallbackToFile("index.html");
            });
        }

        #region Config
        /// <summary>
        /// 
        /// </summary>
        //private static void LoadConfigsFromEnv()
        //{
        //    DotNetEnv.Env.Load();
        //    mySQLConn = DotNetEnv.Env.GetString("MYSQL_CONNECTION");

        //}
        #endregion
    }
}
