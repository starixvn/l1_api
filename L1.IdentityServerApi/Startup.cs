using System;
using L1.IdentityServerApi.Extensions;
using L1.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

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
            var appSettings = services.AddSettings(Configuration);

            services.AddServices();
            services.AddHealthChecks();

            services.AddIdentityServer(options =>
            {
                options.Caching.ResourceStoreExpiration = TimeSpan.FromSeconds(double.Parse(Configuration["IdentityResourceCaching"]));
                options.Caching.ClientStoreExpiration = TimeSpan.FromSeconds(double.Parse(Configuration["IdentityClientCaching"]));
                options.IssuerUri = Configuration["IdentityServer:IssuerUri"];
            })
            .AddMongoDb(appSettings.MongoDBSettings.ConnectionString)
            .AddIdentityApiResources()
            .AddInMemoryCaching()
            .AddIdentityClients()
            .AddSigningCredentialStore();

            IdentityModelEventSource.ShowPII = true;

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            app.UseHttpsRedirection();

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false,
            };

            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler();
            
            app.UseAuthorization();
            
            app.UseIdentityServer();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}