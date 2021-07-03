using System.Net.Http;
using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.BackendAuthentications
{
    public static class BackendAuthMiddleware
    {
        public static void AddBackendAuthService(this IServiceCollection services, BackendAuthConfigs backendAuthConfigs)
        {
            services.AddSingleton(backendAuthConfigs);
            services.AddHttpClient<IBackendAuthService, BackendAuthService>();
            services.AddSingleton<IDiscoveryCache>(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                return new DiscoveryCache(backendAuthConfigs.Address, () => factory.CreateClient(), new DiscoveryPolicy
                {
                    RequireHttps = false
                });
            });                      
        }
    }
}