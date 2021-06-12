using HVMDash.Server.Context;
using HVMDash.Server.Data;
using HVMDash.Server.Hubs;
using HVMDash.Server.Models;
using HVMDash.Server.Service;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
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

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("MSSQL_UserConnection")));

            services.AddDbContext<PlaylistContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<ConsolePhotostockContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<PostedTracksContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<ParserXpathContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));
            services.AddDbContext<PostContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("MSSQL")));

            services.AddMemoryCache();
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            ); //N+1

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(
                //options =>
                //{
                //    // Clients
                //    var spaClient = ClientBuilder
                //        .SPA("HVMDash.Client")
                //        .WithRedirectUri("https://...")
                //        .WithLogoutRedirectUri("https://...")
                //        .WithScopes("...")
                //        .Build();
                //    spaClient.AllowedCorsOrigins = new[]
                //    {
                //        "https://hvm2.kiselevus.keenetic.pro",
                //        "https://localhost:443"
                //    };

                //    options.Clients.Add(spaClient);
                //}
                );

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddCors();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddHostedService<TimedHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<CommandsHub>("/commandshub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
