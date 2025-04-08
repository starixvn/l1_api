using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

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