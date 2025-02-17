using System;
using L1.Features.BackendAuthentications;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.Partners;
using L1.Features.Sites;
using L1.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NATS.Client;
using NATS.Client.JetStream;

namespace L1.Shared.Middlewares
{
    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddMemoryCache(options =>
            {
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
            });
            services.AddSingleton<ICacheManager, CacheManager>();
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

            var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("L1"));
            BsonSerializer.RegisterSerializer(objectSerializer);
        }

        public static void AddJetStreamServices(this IServiceCollection services, IAppSettings appSettings)
        {
            services.AddSingleton<IConnection>(sp =>
            {
                var opts = ConnectionFactory.GetDefaultOptions();
                opts.Url = appSettings.StreamSettings.Host;
                return new ConnectionFactory().CreateConnection(opts);
            });

            services.AddSingleton<INatsService, NatsService>();
        }
    }
}