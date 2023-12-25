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
            services.AddHttpClient<ISabaPlayService, SabaPlayService>(httpClient =>
            {
                httpClient.BaseAddress = SabaPlayServiceSettings.BaseUrl;
            });
        }
    }
}