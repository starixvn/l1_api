using System;
using System.Text.Json;
using L1.Features.BackendAuthentications;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.Partners;
using L1.Features.Sites;
using L1.Shared.Configurations;
using L1.Shared.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NATS.Client;
using NATS.Client.Internals;
using NATS.Client.JetStream;
using Sentry;

namespace L1.Shared.Middlewares
{
    public static class ServiceMiddleware
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton(new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            services.AddSingleton<IJsonService, JsonService>();
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
            services.AddScoped<IPartnerPlayerIdConverter, PartnerPlayerIdConverter>();

            services.AddScoped<ILogDataAccess, LogDataAccess>();
            services.AddScoped<ILogService, LogService>();

            services.AddHttpClient<ProtectedApiBearerTokenHandler>();
        }

        public static void AddMongoDBServices(this IServiceCollection services, IAppSettings appSettings)
        {
            var camelIgnoringExtraElementConventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register(nameof(camelIgnoringExtraElementConventions), camelIgnoringExtraElementConventions, t => true);
            var clientSettings = MongoClientSettings.FromConnectionString(appSettings.MongoDBSettings.ConnectionString);

#pragma warning disable CS0618 // Type or member is obsolete
            //TODO we will use V3 after transforming our query to builder
            clientSettings.LinqProvider = LinqProvider.V2;
#pragma warning restore CS0618 // Type or member is obsolete

            services.AddSingleton<IMongoClient>(sp => new MongoClient(clientSettings));

            var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("L1"));
            BsonSerializer.TryRegisterSerializer(objectSerializer);
        }

        public static void AddJetStreamServices(this IServiceCollection services, IAppSettings appSettings)
        {
            services.AddSingleton<IConnection>(sp =>
            {
                var opts = ConnectionFactory.GetDefaultOptions();
                opts.Url = appSettings.StreamSettings.Host;
                var connection = new ConnectionFactory().CreateConnection(opts);

                var sentryClient = sp.GetRequiredService<ISentryClient>();
                InitializeJetStream(connection, appSettings, sentryClient);

                return connection;
            });

            services.AddSingleton<INatsService, NatsService>();
        }

        private static void InitializeJetStream(IConnection connection, IAppSettings appSettings, ISentryClient sentryClient)
        {
            try
            {
                var settings = appSettings.StreamSettings;
                var jsm = connection.CreateJetStreamManagementContext();

                var streamConfig = StreamConfiguration.Builder()
                    .WithName($"{settings.Prefix}_{settings.StreamName}")
                    .WithSubjects($"{settings.SubjectPrefix}.>")
                    .WithRetentionPolicy(settings.GetRetentionPolicy())
                    .WithDiscardPolicy(settings.GetDiscardPolicy())
                    .WithMaxAge(Duration.OfHours(settings.MaxAgeHours))
                    .WithDuplicateWindow(Duration.OfMinutes(settings.DuplicateWindowMinutes))
                    .Build();

                jsm.AddStream(streamConfig);
            }
            catch (NATSJetStreamException ex) when (ex.ErrorCode == 10058)
            {
                sentryClient.CaptureMessage($"Stream {appSettings.StreamSettings.Prefix} already exists");
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
            }
        }
    }
}