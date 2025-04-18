using L1.Features.PartnerCommunicators.EndGame;
using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.PartnerCommunicators
{
    public static class PartnerCommunicatorMiddleware
    {
        public static void AddPartnerCommunicatorService(this IServiceCollection services)
        {
            services.AddScoped<IPartnerEndGameHandler, PartnerEndGameHandler>();
        }
    }
}