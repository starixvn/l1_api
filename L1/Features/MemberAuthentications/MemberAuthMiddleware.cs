using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.MemberAuthentications
{
    public static class MemberAuthMiddleware
    {
        public static void AddMemberAuthentications(this IServiceCollection services)
        {
            services.AddScoped<IMemberAuthDataAccess, MemberAuthDataAccess>();
            services.AddScoped<IMemberAuthService, MemberAuthService>();
        }
    }
}