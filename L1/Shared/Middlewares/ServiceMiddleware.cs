using System;
using Fanex.Data.Repository;
using L1.Features.Authentications;
using L1.Features.Sites;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
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
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
            var client = new MongoClient(appSettings.MongoDBSettings.ConnectionString);

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton<IMemberAuthDataAccess, MemberAuthDataAccess>();
            services.AddSingleton<IMemberAuthService, MemberAuthService>();
            services.AddSingleton<ISiteDataAccess, SiteDataAccess>();
            services.AddSingleton<ISiteDataService, SiteDataService>();
        }
    }
}