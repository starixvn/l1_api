using L1.Features.BackendAuthentications;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.AdminApiCommunicators
{
    public static class AdminApiMiddleware
    {
        public static void AddAdminApiServices(this IServiceCollection services, AdminApiSettings adminApiSettings)
        {
            services.AddSingleton(adminApiSettings.Endpoints);
            services
                .AddHttpClient<IAdminApiService, AdminApiService>(httpClient =>
                {
                    httpClient.BaseAddress = adminApiSettings.BaseUrl;
                })
                .AddHttpMessageHandler<ProtectedApiBearerTokenHandler>();
        }
    }
}