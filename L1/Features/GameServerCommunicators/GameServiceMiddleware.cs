using L1.Features.IdentityClient;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.GameServerCommunicators
{
    public static class GameServiceMiddleware
    {
        public static void AddGameServerServices(this IServiceCollection services, GameServerSetting gameServerSetting)
        {
            services.AddSingleton(gameServerSetting.Endpoints);
            services.AddScoped<IGameMemberService, GameMemberService>();

            services
                .AddHttpClient<IGameServerService, GameServerService>(httpClient =>
                {
                    httpClient.BaseAddress = gameServerSetting.BaseUrl;
                })
                .AddHttpMessageHandler<ProtectedApiBearerTokenHandler>();
        }
    }
}