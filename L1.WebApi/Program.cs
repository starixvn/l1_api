using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace L1.WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Add configuration
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Add Sentry
            builder.WebHost.UseSentry(opt =>
                opt.Environment = builder.Configuration["Env"]);

            // Configure port
            builder.WebHost.UseUrls($"http://0.0.0.0:{builder.Configuration["AppSettings:HostingPort"]}");

            // Add services from Startup.cs
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            // Configure middleware from Startup.cs
            startup.Configure(app, app.Environment);

            app.Run();
        }
    }
}