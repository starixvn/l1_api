using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.BackendAuthentications
{
    public static class BackendAuthMiddleware
    {
        public static void AddBackendAuthService(this IServiceCollection services, BackendAuthConfigs backendAuthConfigs)
        {
            services.AddSingleton(backendAuthConfigs);
            services.AddHttpClient<IBackendAuthService, BackendAuthService>();
        }
    }
}