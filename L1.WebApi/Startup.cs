using System;
using System.Collections.Generic;
using Coravel;
using L1.Features.AdminApiCommunicators;
using L1.Features.BackendAuthentications;
using L1.Features.GameConfigurations;
using L1.Features.HotdogCommunicators;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PFFCommunicators;
using L1.Features.SabaPlayCommunicators;
using L1.Features.SboCommunicators;
using L1.Features.SGCommunicators;
using L1.Shared.Configurations;
using L1.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
            var appSettings = services.AddSettings(Configuration);

            services.AddBackendAuthService(appSettings.BackendAuthConfigs);
            services.AddServices();
            services.AddMongoDBServices(appSettings);
            services.AddMemberAuthentications();
            services.AddAdminApiServices(appSettings.AdminApiSettings);
            services.AddPartnerCommunicatorService();
            services.AddConfigurationApiServices(appSettings.ConfigurationApiSettings);
            services.AddJetStreamServices(appSettings);

            var serviceRegistrations = new List<(object Settings, Action Registration)>
            {
                (appSettings.SGServiceSettings, () => services.AddSGServices(appSettings.SGServiceSettings)),
                (appSettings.OWServiceSettings, () => services.AddOWServices(appSettings.OWServiceSettings)),
                (appSettings.SboServiceSettings, () => services.AddSboServices(appSettings.SboServiceSettings)),
                (appSettings.PFFServiceSettings, () => services.AddPFFServices(appSettings.PFFServiceSettings)),
                (appSettings.SabaPlayServiceSettings, () => services.AddSabaPlayServices(appSettings.SabaPlayServiceSettings)),
                (appSettings.SabaPlayDailySummaryServiceSettings, () => services.AddSabaPlayDailySummaryServices(appSettings.SabaPlayDailySummaryServiceSettings))
            };

            foreach (var (settings, registration) in serviceRegistrations)
            {
                if (settings != null)
                {
                    registration();
                }
            }

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
            });

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SC API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || Configuration["Env"].Contains("LOCAL"))

            {
                app.UseSwagger();
                app.UseReDoc(c =>
                {
                    c.DocumentTitle = "SC API Documentation";
                });
            }

            ConfigureScheduler(app);

            app.ConfigureExceptionHandler();
            app.ConfigureLocalization();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Sentry.SentrySdk.CaptureMessage($"Game API Started at {DateTimeOffset.Now}");
        }

        private static void ConfigureScheduler(IApplicationBuilder app)
        {
            var serviceProvider = app.ApplicationServices;
            var appSettings = serviceProvider.GetService<IAppSettings>();

            if (appSettings.SabaPlayDailySummaryServiceSettings != null)
            {
                Sentry.SentrySdk.CaptureMessage("Setup Sabaplay Scheule Jobs");
                serviceProvider.UseScheduler(scheduler => scheduler.AddSabaplayScheduleJobs());
            }
        }
    }
}