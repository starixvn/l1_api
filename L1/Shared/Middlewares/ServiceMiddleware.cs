using L1.Features.BackendAuthentications;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.Partners;
using L1.Features.Sites;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

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

            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(appSettings.MongoDBSettings.ConnectionString));

            clientSettings.ClusterConfigurator = cb => cb.Subscribe(new DiagnosticsActivityEventSubscriber());
            services.AddSingleton<IMongoClient>(new MongoClient(clientSettings));
        }
    }
}