using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using L1.Features.PartnerCommunicators;
using L1.Features.SabaPlayCommunicators.CheckMaintenance;

namespace L1.Features.SabaPlayCommunicators
{
    public static class SabaPlayServiceMiddleware
    {
        public static void AddSabaPlayServices(this IServiceCollection services, SabaPlayServiceSettings sabaPlayServiceSettings)
        {
            services.AddSingleton(sabaPlayServiceSettings);
            services.AddSingleton(sabaPlayServiceSettings.Endpoints);
            services.AddScoped<ISabaPlayFailureHandler, SabaPlayFailureHandler>();

            services.AddScoped<SabaPlayCustomerService>()
                   .AddScoped<IPartnerCustomerServices, SabaPlayCustomerService>(s => s.GetService<SabaPlayCustomerService>());

            services.AddHttpClient<ISabaPlayMaintenanceService, SabaPlayMaintenanceService>();

            services.AddHttpClient<ISabaPlayService, SabaPlayService>(httpClient =>
            {
                httpClient.BaseAddress = sabaPlayServiceSettings.BaseUrl;
                httpClient.Timeout = TimeSpan.FromSeconds(30); // Add reasonable timeout
            });
        }
    }
}