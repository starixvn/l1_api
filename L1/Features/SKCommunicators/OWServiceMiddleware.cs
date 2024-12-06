using System;
using System.Net;
using System.Net.Http;
using L1.Features.SKCommunicators.CheckMaintenance;
using L1.Features.PartnerCommunicators;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.SKCommunicators
{
    public static class SKServiceMiddleware
    {
        public static void AddSKServices(this IServiceCollection services, SKServiceSettings skServiceSettings)
        {
            services.AddSingleton(skServiceSettings);
            services.AddSingleton(skServiceSettings.Endpoints);
            services.AddScoped<ISKFailureHandler, SKFailureHandler>();

            services.AddScoped<SKCustomerService>()
                        .AddScoped<IPartnerCustomerServices, SKCustomerService>(s => s.GetService<SKCustomerService>());

            services.AddHttpClient<ISKMaintenanceService, SKMaintenanceService>();
            var skHttpClient = services.AddHttpClient<ISKService, SKService>(httpClient =>
            {
                httpClient.BaseAddress = skServiceSettings.BaseUrl;
            });

            if (!string.IsNullOrEmpty(skServiceSettings.Proxy))
            {
                skHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(skServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }
    }
}