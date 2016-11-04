using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            }

            services.AddDbContext<WorldContext>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddTransient<GeoCoordsService>();
            
            services.AddTransient<WorldContextSeedData>();
            services.AddLogging();
            
            services.AddMvc()
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
            Mapper.Initialize(config =>
            {
                config.CreateMap<TripViewModel, Trip>().ReverseMap();
                config.CreateMap<StopViewModel, Stop>().ReverseMap();
            });

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

            app.UseNodeModules(env.ContentRootPath);

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
