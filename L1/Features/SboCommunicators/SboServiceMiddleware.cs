using L1.Features.Partners;
using L1.Features.SboCommunicators.CheckMaintenance;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.SboCommunicators
{
    public static class SboServiceMiddleware
    {
        public static void AddSboServices(this IServiceCollection services, SboServiceSettings SboServiceSettings)
        {
            services.AddSingleton(SboServiceSettings);
            services.AddSingleton(SboServiceSettings.Endpoints);
            services.AddScoped<ISboFailureHandler, SboFailureHandler>();

            services.AddScoped<SboCustomerService>()
                        .AddScoped<IPartnerCustomerServices, SboCustomerService>(s => s.GetService<SboCustomerService>());

            services.AddHttpClient<IMaintenanceService, MaintenanceService>();
            services.AddHttpClient<ISboService, SboService>(httpClient =>
            {
                httpClient.BaseAddress = SboServiceSettings.BaseUrl;
            });
        }
    }
}