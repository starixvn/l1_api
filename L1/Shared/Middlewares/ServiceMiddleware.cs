using System;
using Fanex.Data.Repository;
using L1.Features.Authentications;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace L1.Shared.Middlewares
{
    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();

            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);
        }

        public static void AddMongoDBServices(this IServiceCollection services, AppSettings appSettings)
        {
            var client = new MongoClient(appSettings.MongoDBSettings.ConnectionString);
            var database = client.GetDatabase(appSettings.MongoDBSettings.DatabaseName);

            services.AddSingleton(database);
            services.AddSingleton<IMemberAuthDataAccess, MemberAuthDataAccess>();
            services.AddSingleton<IMemberAuthService, MemberAuthService>();
        }
    }
}