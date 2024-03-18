using System.Net;
using System.Net.Http;
using L1.Features.PartnerCommunicators;
using L1.Features.SabaPlayCommunicators.CheckMaintenance;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.SabaPlayCommunicators
{
    public static class SabaPlayServiceMiddleware
    {
        public static void AddSabaPlayServices(this IServiceCollection services, SabaPlayServiceSettings SabaPlayServiceSettings)
        {
            services.AddSingleton(SabaPlayServiceSettings);
            services.AddSingleton(SabaPlayServiceSettings.Endpoints);
            services.AddScoped<ISabaPlayFailureHandler, SabaPlayFailureHandler>();

            services.AddScoped<SabaPlayCustomerService>()
                        .AddScoped<IPartnerCustomerServices, SabaPlayCustomerService>(s => s.GetService<SabaPlayCustomerService>());

            services.AddHttpClient<ISabaPlayMaintenanceService, SabaPlayMaintenanceService>();
            var sabaPlayHttpClient = services.AddHttpClient<ISabaPlayService, SabaPlayService>(httpClient =>
            {
                httpClient.BaseAddress = SabaPlayServiceSettings.BaseUrl;
            });

            if (!string.IsNullOrEmpty(SabaPlayServiceSettings.Proxy))
            {
                sabaPlayHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(SabaPlayServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }
    }
}