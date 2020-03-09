using System;
using Fanex.Logging;
using L1.Configurations;
using L1.WebApi.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace L1.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddLogging(Configuration);
                var appSettings = services.AddSettings(Configuration);
                services.AddServices();

                var builder = services.AddIdentityServer()
                   .AddInMemoryApiResources(IdentityConfigs.Apis)
                   .AddInMemoryClients(IdentityConfigs.Clients);

                builder.AddDeveloperSigningCredential();

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
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.ConfigureExceptionHandler();
                app.UseDatabase(Configuration);

                app.UseRouting();

                app.UseAuthorization();

                app.UseIdentityServer();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGet("/", context => context.Response.WriteAsync("Site is running"));
                });

                //app.UsePathBase(Configuration["HostingVirtualPath"]);
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }

            LogService.Info("LiveScore API has been started.");
        }
    }
}