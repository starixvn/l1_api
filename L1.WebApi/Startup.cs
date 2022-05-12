using L1.Features.AdminApiCommunicators;
using L1.Features.BackendAuthentications;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Features.SboCommunicators;
using L1.Features.SGApiCommunicators;
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
using OpenTelemetry.Metrics;
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
            services.AddSGApiServices(appSettings.SGApiSettings);

            if (appSettings.SboServiceSettings != null)
            {
                services.AddSboServices(appSettings.SboServiceSettings);
            }

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

            if (this.Configuration.GetSection("Jaeger") != null)
            {
                services.AddOpenTelemetryTracing((builder) => builder
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(this.Configuration.GetValue<string>("Jaeger:ServiceName")))
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddMongoDBInstrumentation()
                            .AddJaegerExporter());

                services.Configure<JaegerExporterOptions>(this.Configuration.GetSection("Jaeger"));

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}