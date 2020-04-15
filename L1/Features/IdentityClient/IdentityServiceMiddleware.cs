using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.IdentityClient
{
    public static class IdentityServiceMiddleware
    {
        public static void AddIdentityService(this IServiceCollection services, IdentityServerConfigs identityServerConfigs)
        {
            services.AddSingleton(identityServerConfigs);
            services.AddHttpClient<IIdentityService, IdentityService>();
        }
    }
}