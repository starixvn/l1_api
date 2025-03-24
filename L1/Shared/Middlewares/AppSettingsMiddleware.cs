using System;
using L1.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Shared.Middlewares
{
    public static class AppSettingsMiddleware
    {
        public static AppSettings AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>()
                ?? throw new InvalidOperationException("AppSettings section is missing in configuration");
            
            services.AddSingleton<IAppSettings>(appSettings);

            return appSettings;
        }
    }
}