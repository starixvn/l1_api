using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.MemberAuthentications
{
    public static class MemberAuthMiddleware
    {
        public static void AddMemberAuthentications(this IServiceCollection services)
        {
            services.AddSingleton<IMemberAuthDataAccess, MemberAuthDataAccess>();
            services.AddSingleton<IMemberAuthService, MemberAuthService>();
        }
    }
}