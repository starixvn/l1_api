using System;
using L1.Features.AdminApiCommunicators;
using L1.Features.BackendAuthentications;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Features.PFFCommunicators;
using L1.Features.SabaPlayCommunicators;
using L1.Features.SboCommunicators;
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
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
            services.AddOWServices(appSettings.OWServiceSettings);
            services.AddAdminApiServices(appSettings.AdminApiSettings);

            if (appSettings.SboServiceSettings != null)
            {
                services.AddSboServices(appSettings.SboServiceSettings);
            }

            if (appSettings.PFFServiceSettings != null)
            {
                services.AddPFFServices(appSettings.PFFServiceSettings);
            }

            if (appSettings.SabaPlayServiceSettings != null)
            {
                services.AddSabaPlayServices(appSettings.SabaPlayServiceSettings);
            }

            services.AddControllers();

            services.AddMvc();
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
            });

            if (Configuration.GetSection("Jaeger") != null)
            {
                services.AddOpenTelemetryTracing((builder) => builder
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(this.Configuration.GetValue<string>("Jaeger:ServiceName")))
                            .AddAspNetCoreInstrumentation((options) => options.Filter = httpContext =>
                            {
                                // only collect telemetry about HTTP POST requests
                                return httpContext.Request.Method.Equals("POST");
                            })
                            .AddHttpClientInstrumentation()
                            .AddMongoDBInstrumentation()
                            .AddJaegerExporter());

                services.Configure<JaegerExporterOptions>(Configuration.GetSection("Jaeger"));

                //// Customize the HttpClient that will be used when JaegerExporter is configured for HTTP transport.
                services.AddHttpClient("JaegerExporter", configureClient: (client) => client.DefaultRequestHeaders.Add("X-MyCustomHeader", "value"));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseReDoc(c =>
                {
                    c.DocumentTitle = "SC API Documentation";
                    c.SpecUrl = "/swagger/v1/swagger.json";
                });
            }

            app.ConfigureExceptionHandler();
            app.ConfigureLocalization();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            Sentry.SentrySdk.CaptureMessage($"Game API Started at {DateTime.Now}");
        }
    }
}