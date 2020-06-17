using L1.Features.BackendAuthentications;
using L1.Features.Sites;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace L1.Shared.Middlewares
{
    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ISiteDataAccess, SiteDataAccess>();
            services.AddSingleton<ISiteDataService, SiteDataService>();

            services.AddTransient<ProtectedApiBearerTokenHandler>();
        }

        public static void AddMongoDBServices(this IServiceCollection services, IAppSettings appSettings)
        {
            var camelIgnoringExtraElementConventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register(nameof(camelIgnoringExtraElementConventions), camelIgnoringExtraElementConventions, t => true);
            services.AddSingleton<IMongoClient>(new MongoClient(appSettings.MongoDBSettings.ConnectionString));
        }
    }
}