using System;
using Fanex.Logging;
using L1.Configurations;
using L1.Jobs.Middlewares;
using L1.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace L1.Jobs
{
    public class Startup
    {
        private AppSettings appSettings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddLogging(Configuration);
                appSettings = services.AddSettings(Configuration);

                services.AddServices();

                services.AddControllers();
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            try
            {
                app.UseHangfire(appSettings);

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.ConfigureExceptionHandler();
                app.UseDatabase(Configuration);

                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Hello World!");
                    });
                });
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }

            LogService.Info("Site has been started.");
        }
    }
}