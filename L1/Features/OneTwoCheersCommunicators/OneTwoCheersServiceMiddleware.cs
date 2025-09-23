namespace L1.Features.OneTwoCheersCommunicators
{
    using System;
    using L1.Features.PartnerCommunicators;
    using Microsoft.Extensions.DependencyInjection;
    using L1.Features.OneTwoCheersCommunicators.CheckMaintenance;
    using System.Net.Http;
    using System.Net;
    public static class OneTwoCheersServiceMiddleware
    {
        public static void AddOneTwoCheersServices(this IServiceCollection services, OneTwoCheersServiceSettings oneTwoCheersServiceSettings)
        {
            services.AddSingleton(oneTwoCheersServiceSettings);
            services.AddSingleton(oneTwoCheersServiceSettings.Endpoints);
            services.AddScoped<IOneTwoCheersFailureHandler, OneTwoCheersFailureHandler>();

            services.AddScoped<OneTwoCheersCustomerService>()
                   .AddScoped<IPartnerCustomerServices, OneTwoCheersCustomerService>(s => s.GetService<OneTwoCheersCustomerService>());

            services.AddHttpClient<IOneTwoCheersMaintenanceService, OneTwoCheersMaintenanceService>();

            var owHttpClient = services.AddHttpClient<IOneTwoCheersService, OneTwoCheersService>(httpClient =>
            {
                httpClient.BaseAddress = oneTwoCheersServiceSettings.BaseUrl;
                httpClient.Timeout = TimeSpan.FromSeconds(30); // Add reasonable timeout
            });

            if (!string.IsNullOrEmpty(oneTwoCheersServiceSettings.Proxy))
            {
                owHttpClient.ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        Proxy = new WebProxy(oneTwoCheersServiceSettings.Proxy),
                        UseProxy = true,
                    };
                });
            }
        }
    }
}
