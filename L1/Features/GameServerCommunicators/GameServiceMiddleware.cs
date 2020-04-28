using L1.Features.BackendAuthentications;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.GameServerCommunicators
{
    public static class GameServiceMiddleware
    {
        public static void AddGameServerServices(this IServiceCollection services, GameServerSettings gameServerSettings)
        {
            services.AddSingleton(gameServerSettings.Endpoints);
            services.AddScoped<IGameMemberService, GameMemberService>();

            services
                .AddHttpClient<IGameServerService, GameServerService>(httpClient =>
                {
                    httpClient.BaseAddress = gameServerSettings.BaseUrl;
                })
                .AddHttpMessageHandler<ProtectedApiBearerTokenHandler>();
        }
    }
}