using L1.Features.BackendAuthentications;
using L1.Features.Partners;
using L1.Features.Partners.Log;
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
            services.AddScoped<ISiteDataAccess, SiteDataAccess>();
            services.AddScoped<ISiteDataService, SiteDataService>();

            services.AddScoped<IPartnerDataAccess, PartnerDataAccess>();
            services.AddScoped<ICurrencyDataAccess, CurrencyDataAccess>();
            services.AddScoped<IPartnerDataService, PartnerDataService>();
            services.AddScoped<IPartnerCustomerFactory, PartnerCustomerFactory>();

            services.AddScoped<ILogDataAccess, LogDataAccess>();
            services.AddScoped<ILogService, LogService>();

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