using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.Log;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Shared.Middlewares
{
    public static class OWServiceMiddleware
    {
        public static void AddOWServices(this IServiceCollection services, OWServiceSettings owServiceSettings)
        {
            services.AddSingleton(new OWAuth(owServiceSettings.Login, owServiceSettings.Password));
            services.AddSingleton(owServiceSettings.Endpoints);
            services.AddScoped<ILogDataAccess, LogDataAccess>();
            services.AddScoped<IOWCustomerService, OWCustomerService>();

            services.AddHttpClient<IOWFailureHandler, OWFailureHandler>(httpClient =>
            {
                httpClient.BaseAddress = owServiceSettings.BaseUrl;
            });

            services.AddHttpClient<IOWService, OWService>(httpClient =>
            {
                httpClient.BaseAddress = owServiceSettings.BaseUrl;
            });
        }
    }
}