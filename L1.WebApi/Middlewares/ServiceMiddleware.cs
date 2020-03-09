using System;
using Fanex.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace L1.WebApi.Middlewares
{
    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);
        }
    }
}