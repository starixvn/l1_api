using System;
using System.Collections.Generic;
using Fanex.Logging;
using L1.Features.Sites;
using L1.Shared.Configurations;
using L1.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace L1.IdentityServerApi
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
            try
            {
                services.AddLogging(Configuration);
                var appSettings = services.AddSettings(Configuration);
                services.AddServices(appSettings);

                var sites = getAllSites(appSettings);
                var builder = services.AddIdentityServer()
                   .AddInMemoryApiResources(IdentityConfigs.Apis(sites))
                   .AddInMemoryClients(IdentityConfigs.Clients(sites));
                builder.AddDeveloperSigningCredential();

                services.AddControllers();
                services.AddMvc();
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

                app.UseRouting();

                //app.UseForwardedHeaders(new ForwardedHeadersOptions
                //{
                //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                //});

                app.UseAuthorization();
                app.UseIdentityServer();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
            }
            catch (Exception ex)
            {
                LogService.Error(ex.ToString());
            }

            LogService.Info("Auth Server has been started.");
        }

        private IEnumerable<Site> getAllSites(AppSettings appSettings)
        {
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
            var client = new MongoClient(appSettings.MongoDBSettings.ConnectionString);

            return new SiteDataAccess(client).GetAll();
        }

    }
}