namespace OW.ApiSimulator.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using OW.ApiSimulator.Models.OW;

    public static class SimulatorExtensions
    {
        public static void AddOW(this IServiceCollection services)
        {
            services.AddSingleton(new NotfityCustomerLoginResult());
            services.AddSingleton(new GetBalanceResult());
            services.AddSingleton(new PlaceBetResult());
            services.AddSingleton(new EndGameResult());
            services.AddSingleton(new VoidGameResult());
        }
    }
}
