using L1.Features.OWCommunicators;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Shared.Middlewares
{
    public static class OWServiceMiddleware
    {
        public static void AddOWServices(this IServiceCollection services, OWServiceSettings owServiceSettings)
        {
            services.AddSingleton<IOWAuth>(new OWAuth(owServiceSettings.Login, owServiceSettings.Password));

            services.AddHttpClient<IOWService>(httpClient =>
            {
                httpClient.BaseAddress = owServiceSettings.BaseUrl;
            });
        }
    }
}