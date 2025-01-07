using L1.Features.OWCommunicators;
using System.Net.Http;
using L1.Features.PartnerCommunicators;
using L1.Features.PFFCommunicators.CheckMaintenance;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace L1.Features.PFFCommunicators
{
    public static class PFFServiceMiddleware
    {
        public static void AddPFFServices(this IServiceCollection services, PFFServiceSettings PFFServiceSettings)
        {
            services.AddSingleton(PFFServiceSettings);
            services.AddSingleton(PFFServiceSettings.Endpoints);
            services.AddScoped<IPFFFailureHandler, PFFFailureHandler>();

            services.AddScoped<PFFCustomerService>()
                        .AddScoped<IPartnerCustomerServices, PFFCustomerService>(s => s.GetService<PFFCustomerService>());

            services.AddHttpClient<IPFFMaintenanceService, PFFMaintenanceService>();

            var pffHttpClient = services.AddHttpClient<IPFFService, PFFService>(httpClient =>
            {
                httpClient.BaseAddress = PFFServiceSettings.BaseUrl;
            });

            if (!string.IsNullOrEmpty(PFFServiceSettings.Proxy))
            {
                pffHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(PFFServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }
    }
}