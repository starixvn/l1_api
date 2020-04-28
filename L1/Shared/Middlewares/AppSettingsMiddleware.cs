using L1.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Shared.Middlewares
{
    public static class AppSettingsMiddleware
    {
        public static AppSettings AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = new AppSettings();

            configuration.GetSection("AppSettings").Bind(appSettings);
            services.AddSingleton<IAppSettings>(appSettings);

            return appSettings;
        }
    }
}