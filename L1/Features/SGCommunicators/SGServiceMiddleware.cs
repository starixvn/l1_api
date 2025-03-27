using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using L1.Features.PartnerCommunicators;

namespace L1.Features.SGCommunicators
{
    public static class SGServiceMiddleware
    {
        public static void AddSGServices(this IServiceCollection services, SGCommunicatorSettings settings)
        {
            services.AddSingleton(settings);
            services.AddSingleton(settings.Endpoints);
            services.AddScoped<ISGFailureHandler, SGFailureHandler>();

            services.AddScoped<SGCustomerService>()
                   .AddScoped<IPartnerCustomerServices, SGCustomerService>(s => s.GetService<SGCustomerService>());

            var sgHttpClient = services.AddHttpClient<ISGService, SGService>(httpClient =>
            {
                httpClient.BaseAddress = settings.BaseUrl;
                httpClient.Timeout = TimeSpan.FromSeconds(30); // Add reasonable timeout
            });

            if (!string.IsNullOrEmpty(settings.Proxy))
            {
                sgHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(settings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }
    }
} 