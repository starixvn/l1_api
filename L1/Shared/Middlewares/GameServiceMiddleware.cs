using L1.Features.GameServerCommunicators;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Shared.Middlewares
{
    public static class GameServiceMiddleware
    {
        public static void AddGameServerServices(this IServiceCollection services, GameServerSetting gameServerSetting)
        {
            services.AddSingleton(gameServerSetting.Endpoints);
            services.AddScoped<IGameMemberService, GameMemberService>();

            services.AddHttpClient<IGameServerService, GameServerService>(httpClient =>
            {
                httpClient.BaseAddress = gameServerSetting.BaseUrl;
            });
        }
    }
}