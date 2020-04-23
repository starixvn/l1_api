using L1.Features.AdminApiCommunicators;
using L1.Features.BackendAuthentications;
using L1.Features.GameServerCommunicators;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
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
                var appSettings = services.AddSettings(Configuration);

                services.AddBackendAuthService(appSettings.BackendAuthConfigs);
                services.AddServices();
                services.AddMongoDBServices(appSettings);
                services.AddMemberAuthentications();
                services.AddOWServices(appSettings.OWServiceSettings);
                services.AddGameServerServices(appSettings.GameServerSettings);
                services.AddAdminApiServices(appSettings.AdminApiSettings);

                services.AddControllers();

                services
                    .AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = appSettings.BackendAuthConfigs.Address;
                        options.RequireHttpsMetadata = false;

                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidAudiences = appSettings.IdentityAudiences
                        };
                    });

                services.AddMvc();
            }
            catch
            {
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler();
            app.ConfigureLocalization();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}