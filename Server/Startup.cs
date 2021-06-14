using HVMDash.Server.Context;
using HVMDash.Server.Data;
using HVMDash.Server.Hubs;
using HVMDash.Server.Models;
using HVMDash.Server.Service;
using IdentityServer4.Services;
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
using Microsoft.Extensions.Logging;
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

            services.AddIdentityServer(
                                //options =>
                                //{
                                //    options.IssuerUri = Configuration.GetConnectionString("HostAddress");
                                //}
                                /// Added new
                                options =>
                                 {
                                     options.Events.RaiseErrorEvents = true;
                                     options.Events.RaiseInformationEvents = true;
                                     options.Events.RaiseFailureEvents = true;
                                     options.Events.RaiseSuccessEvents = true;

                                     options.Cors.CorsPolicyName = "IdentityServer";

                                     // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                                     options.EmitStaticAudienceClaim = true;
                                 }
                                )
                //Old One
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt()
                .AddGoogle(o =>
                    {
                        o.ClientId = Configuration["Authentication:Google:ClientId"];
                        o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    });

            services.AddControllersWithViews();
            services.AddRazorPages();

            // Added new
            services.AddCors(options =>
            {
                options.AddPolicy("IdentityServer", policy =>
                {
                    policy
                      .AllowAnyOrigin()
                      //.WithOrigins(Configuration.GetConnectionString("HostAddress"))
                      //.SetIsOriginAllowedToAllowWildcardSubdomains()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                });

                options.DefaultPolicyName = "IdentityServer";
            });

            //// Added new
            //services.AddSingleton<ICorsPolicyService>((container) => {
            //    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
            //    return new DefaultCorsPolicyService(logger)
            //    {
            //        AllowAll = true
            //    };
            //});

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

            //New
            // @see https://github.com/dotnet/aspnetcore/issues/16672
            app.UseCors("IdentityServer");

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
