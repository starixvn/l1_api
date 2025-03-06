using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.GameConfigurations
{
    public static class ConfigurationApiMiddleware
    {
        public static void AddConfigurationApiServices(this IServiceCollection services, ConfigurationApiSettings settings)
        {
            services.AddSingleton(settings.Endpoints);
            services
                .AddHttpClient<IConfigurationApiService, ConfigurationApiService>(httpClient =>
                {
                    httpClient.BaseAddress = settings.BaseUrl;
                });
        }
    }
}