using System.Net;
using System.Net.Http;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.PartnerCommunicators;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.OWCommunicators
{
    public static class OWServiceMiddleware
    {
        public static void AddOWServices(this IServiceCollection services, OWServiceSettings owServiceSettings)
        {
            services.AddSingleton(owServiceSettings);
            services.AddSingleton(owServiceSettings.Endpoints);
            services.AddScoped<IOWFailureHandler, OWFailureHandler>();

            services.AddScoped<OWCustomerService>()
                        .AddScoped<IPartnerCustomerServices, OWCustomerService>(s => s.GetService<OWCustomerService>());

            services.AddHttpClient<IOWMaintenanceService, OWMaintenanceService>();
            var owHttpClient = services.AddHttpClient<IOWService, OWService>(httpClient =>
            {
                httpClient.BaseAddress = owServiceSettings.BaseUrl;
            });

            if (!string.IsNullOrEmpty(owServiceSettings.Proxy))
            {
                owHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(owServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }
    }
}