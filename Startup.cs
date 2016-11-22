using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Entities;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            _config = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            if (_env.IsEnvironment("Development") || _env.IsEnvironment("Testing"))
            {
                services.AddScoped<IMailService, DebugMailService>();
            }
            else
            {
                // Implement a real Mail Service
                services.AddScoped<IMailService, DebugMailService>();
            }

            services.AddDbContext<WorldContext>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddTransient<GeoCoordsService>();

            services.AddTransient<WorldContextSeedData>();
            services.AddLogging();
            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = async ctx =>
                      {
                          if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                          {
                              ctx.Response.StatusCode = 401;
                          }
                          else
                          {
                              ctx.Response.Redirect(ctx.RedirectUri);
                          }
                          await Task.Yield();
                      }
                };
            })
            .AddEntityFrameworkStores<WorldContext>();

            // services.ConfigureCookieAuthentication(config =>
            // {
            //     config.LoginPath = "/Auth/Login";
            // });

            services.AddMvc(config =>
            {
                // if (_env.IsProduction())
                // {
                //     config.Filters.Add(new RequireHttpsAttribute());
                // }
            })
            .AddJsonOptions(config =>
            {
                //config.SerializerSettings.ContractResolver = new CamelCasePropertyNameContractResolver();     unnecessary anymore
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            WorldContextSeedData seeder,
            ILoggerFactory factory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);
            }
            else
            {
                // app.UseExceptionHandler(new ExceptionHandlerOptions
                // {
                //     ExceptionHandler = context => context.Response.WriteAsync("Oops!")
                // });
                factory.AddDebug(LogLevel.Error);
            }

            app.UseFileServer();

            app.UseIdentity();

            Mapper.Initialize(config =>
            {
                config.CreateMap<TripViewModel, Trip>().ReverseMap();
                config.CreateMap<StopViewModel, Stop>().ReverseMap();
            });

            app.UseNodeModules(env.ContentRootPath);
            app.UseBowerComponents(env.ContentRootPath);

            // app.UseMvc(config =>
            // {
            //     config.MapRoute(
            //         name: "Default",
            //         template: "{controller}/{action}/{id?}",
            //         defaults: new { controller = "App", action = "Index" }
            //     );
            // });
            app.UseMvc(configureRoutes);

            seeder.EnsureSeedData().Wait();
        }

        private void configureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Default", "{controller=App}/{action=Index}/{id?}");
        }
    }
}
