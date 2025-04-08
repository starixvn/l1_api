using L1.Features.OWCommunicators;
using System.Net.Http;
using L1.Features.PartnerCommunicators;
using L1.Features.HotdogCommunicators.CheckMaintenance;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace L1.Features.HotdogCommunicators
{
    public static class HotdogServiceMiddleware
    {
        public static void AddHotdogServices(this IServiceCollection services, HotdogServiceSettings HotdogServiceSettings)
        {
            services.AddSingleton(HotdogServiceSettings);
            services.AddSingleton(HotdogServiceSettings.Endpoints);
            services.AddScoped<IHotdogFailureHandler, HotdogFailureHandler>();

            services.AddScoped<HotdogCustomerService>()
                        .AddScoped<IPartnerCustomerServices, HotdogCustomerService>(s => s.GetService<HotdogCustomerService>());

            services.AddHttpClient<IHotdogMaintenanceService, HotdogMaintenanceService>();

            var HotdogHttpClient = services.AddHttpClient<IHotdogService, HotdogService>(httpClient =>
            {
                httpClient.BaseAddress = HotdogServiceSettings.BaseUrl;
            });

            if (!string.IsNullOrEmpty(HotdogServiceSettings.Proxy))
            {
                HotdogHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(HotdogServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }
    }
}