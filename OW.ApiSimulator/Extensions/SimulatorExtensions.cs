using Microsoft.Extensions.DependencyInjection;
using OW.ApiSimulator.Models.OW;

namespace OW.ApiSimulator.Extensions
{
    public static class SimulatorExtensions
    {
        public static void AddOW(this IServiceCollection services)
        {
            services.AddSingleton(new EnterPortalResult());
            services.AddSingleton(new GetBalanceResult());
            services.AddSingleton(new PlaceBetResult());
            services.AddSingleton(new EndGameResult());
            services.AddSingleton(new VoidGameResult());
        }
    }
}