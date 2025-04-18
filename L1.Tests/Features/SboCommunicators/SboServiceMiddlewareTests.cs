using System.Collections.Generic;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.BackendAuthentications;
using L1.Features.SboCommunicators;
using L1.Shared.Configurations;
using L1.Shared.Middlewares;
using L1.Tests._Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Xunit;
using System.Text.Json;
using L1.Shared.Json;

namespace L1.Tests.Features.SboCommunicators
{
    public class SboServiceMiddlewareTests
    {
        [Theory, AutoData]
        public void AddSboServices_Always_AddSboServiceSettings(SboServiceSettings SboServiceSettings)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            SboServiceMiddleware.AddSboServices(services, SboServiceSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<SboServiceSettings>();

            // Assert
            Assert.Equal(SboServiceSettings, result);
        }

        [Theory, AutoData]
        public void AddSboServices_Always_AddISboService(SboServiceSettings SboServiceSettings)
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(new Dictionary<string, string>
               {
                    { "AppSettings:MongoDBSettings:ConnectionString", "mongodb://127.0.0.1:27017" }
               })
               .Build();
            services.AddSettings(configuration);
            var appSettings = new AppSettings
            {
                MongoDBSettings = new MongoDBSettings
                {
                    ConnectionString = "mongodb://127.0.0.1:27017"
                },
                AdminApiSettings = new AdminApiSettings
                {
                    Endpoints = new AdminApiEndPoints()
                },
                BackendAuthConfigs = new BackendAuthConfigs(),
                SboServiceSettings = SboServiceSettings
            };

            services.AddBackendAuthService(appSettings.BackendAuthConfigs);
            services.AddServices();
            services.AddMongoDBServices(appSettings);
            services.AddSingleton<ISentryClient, MockSentryClient>();
            services.AddAdminApiServices(appSettings.AdminApiSettings);
            
            // Add JsonService registration
            services.AddSingleton(new JsonSerializerOptions());
            services.AddSingleton<IJsonService, JsonService>();
            services.AddSingleton<IAppSettings>(appSettings);

            // Act
            SboServiceMiddleware.AddSboServices(services, SboServiceSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<ISboService>();

            // Assert
            Assert.NotNull(result);
        }
    }
}