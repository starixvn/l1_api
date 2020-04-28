using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.OWCommunicators.Log;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.OWCommunicators
{
    public static class OWServiceMiddleware
    {
        public static void AddOWServices(this IServiceCollection services, OWServiceSettings owServiceSettings)
        {
            services.AddSingleton(owServiceSettings);
            services.AddSingleton(new OWAuth(owServiceSettings.Login, owServiceSettings.Password));
            services.AddSingleton(owServiceSettings.Endpoints);

            services.AddScoped<ILogDataAccess, LogDataAccess>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IOWFailureHandler, OWFailureHandler>();
            services.AddScoped<IOWCustomerService, OWCustomerService>();

            services.AddHttpClient<IMaintenanceService, MaintenanceService>();
            services.AddHttpClient<IOWService, OWService>(httpClient =>
            {
                httpClient.BaseAddress = owServiceSettings.BaseUrl;
            });
        }
    }
}