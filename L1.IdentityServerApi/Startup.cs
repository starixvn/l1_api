using System;
using L1.IdentityServerApi.Extensions;
using L1.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                var appSettings = services.AddSettings(Configuration);
                services.AddServices(appSettings);

                var builder = services.AddIdentityServer(options =>
                {
                    options.Caching.ResourceStoreExpiration = TimeSpan.FromSeconds(double.Parse(Configuration["IdentityResourceCaching"]));
                    options.Caching.ClientStoreExpiration = TimeSpan.FromSeconds(double.Parse(Configuration["IdentityClientCaching"]));
                })
                .AddMongoDb(appSettings.MongoDBSettings.ConnectionString)
                .AddInMemoryCaching()
                .AddIdentityClients()
                .AddIdentityApiResources()
                .AddDeveloperSigningCredential();

                services.AddControllers();
                services.AddMvc();
            }
            catch (Exception ex)
            {
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
            }
        }
    }
}