using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.SGApiCommunicators
{
    public static class SGApiMiddleware
    {
        public static void AddSGApiServices(this IServiceCollection services, SGApiSettings apiSettings)
        {
            services.AddSingleton(apiSettings.Endpoints);
            services
                .AddHttpClient<ISGApiService, SGApiService>(httpClient =>
                {
                    httpClient.BaseAddress = apiSettings.BaseUrl;
                });
        }
    }
}