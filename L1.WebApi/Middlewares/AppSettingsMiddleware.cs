using L1.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L1.WebApi.Middlewares
{
    public static class AppSettingsMiddleware
    {
        public static IAppSettings AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettings = new AppSettings();

            configuration.GetSection("AppSettings").Bind(appSettings);

            services.AddSingleton<IAppSettings>(appSettings);

            return appSettings;
        }
    }
}