using System;
using L1.Features.BackendAuthentications;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.Sites;
using L1.Shared.Configurations;
using L1.Shared.Json;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using NATS.Client;
using NATS.Client.Internals;
using NATS.Client.JetStream;
using Sentry;

namespace L1.Shared.Middlewares
{
    public static class ServiceMiddleware
    {
        private const int JetStreamErrorStreamAlreadyExists = 10058;

        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IJsonService, JsonService>();
            services.AddMemoryCache(options =>
            {
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
            });
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddScoped<ISiteDataAccess, SiteDataAccess>();
            services.AddScoped<ISiteDataService, SiteDataService>();

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
            services.AddSingleton<IMongoClient>(sp => new MongoClient(appSettings.MongoDBSettings.ConnectionString));

            var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("L1"));
            BsonSerializer.TryRegisterSerializer(objectSerializer);
        }

        public static void AddJetStreamServices(this IServiceCollection services, IAppSettings appSettings)
        {
            services.AddSingleton<IConnection>(sp =>
            {
                if(appSettings.StreamSettings.Enable)
                {
                    var opts = ConnectionFactory.GetDefaultOptions();
                    opts.Url = appSettings.StreamSettings.Host;
                    var connection = new ConnectionFactory().CreateConnection(opts);

                    var sentryClient = sp.GetRequiredService<ISentryClient>();
                    InitializeJetStream(connection, appSettings, sentryClient);

                    return connection;
                }


                return null;
            });

            services.AddSingleton<INatsService, NatsService>();
        }

        private static void InitializeJetStream(IConnection connection, IAppSettings appSettings, ISentryClient sentryClient)
        {
            var formattedStreamName = $"{appSettings.StreamSettings.StreamName}_{appSettings.StreamSettings.Postfix}";

            try
            {
                var settings = appSettings.StreamSettings;
                var jsm = connection.CreateJetStreamManagementContext();

                // Enhanced stream configuration for new streams - supports ack/nak pattern
                var streamConfig = StreamConfiguration.Builder()
                    .WithName(formattedStreamName)
                    .WithSubjects($"{settings.SubjectPrefix}.>")
                    .WithStorageType(ParseStorageType(settings.GetEffectiveStorageType()))
                    .WithRetentionPolicy(ParseRetentionPolicy(settings.GetEffectiveRetentionPolicy()))
                    .WithDiscardPolicy(ParseDiscardPolicy(settings.GetEffectiveDiscardPolicy()))
                    .WithMaxAge(Duration.OfHours(settings.MaxAgeHours))
                    .WithMaxMessages(settings.MaxMessages)
                    .WithMaxBytes(settings.MaxBytes)
                    .WithReplicas(settings.Replicas)
                    .WithDuplicateWindow(Duration.OfMillis(settings.DuplicateWindowMs))
                    .WithNoAck(settings.NoAck)
                    .WithAllowRollup(settings.AllowRollup)
                    .WithDenyDelete(settings.DenyDelete)
                    .WithDenyPurge(settings.DenyPurge)
                    .Build();

                jsm.AddStream(streamConfig);
                sentryClient.CaptureMessage($"Created new stream with ack/nak support: {formattedStreamName}");
            }
            catch (NATSJetStreamException ex) when (ex.ErrorCode == JetStreamErrorStreamAlreadyExists)
            {
                sentryClient.CaptureMessage($"Stream {formattedStreamName} already exists. For migration, see manual steps in documentation.");
            }
            catch (Exception ex)
            {
                sentryClient.CaptureException(ex);
            }
        }

        private static StorageType ParseStorageType(string storageType)
        {
            return storageType?.ToLowerInvariant() switch
            {
                "file" => StorageType.File,
                "memory" => StorageType.Memory,
                _ => StorageType.File
            };
        }

        private static RetentionPolicy ParseRetentionPolicy(string policy)
        {
            return policy?.ToLowerInvariant() switch
            {
                "limits" => RetentionPolicy.Limits,
                "interest" => RetentionPolicy.Interest,
                "workqueue" => RetentionPolicy.WorkQueue,
                _ => RetentionPolicy.Limits
            };
        }

        private static DiscardPolicy ParseDiscardPolicy(string policy)
        {
            return policy?.ToLowerInvariant() switch
            {
                "old" => DiscardPolicy.Old,
                "new" => DiscardPolicy.New,
                _ => DiscardPolicy.Old
            };
        }
    }
}