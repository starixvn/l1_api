using System.Collections.Generic;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.BackendAuthentications;
using L1.Features.PFFCommunicators;
using L1.Shared.Configurations;
using L1.Shared.Middlewares;
using L1.Tests._Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Xunit;

namespace L1.Tests.Features.PFFCommunicators
{
    public class PFFServiceMiddlewareTests
    {
        [Theory, AutoData]
        public void AddPFFServices_Always_AddPFFServiceSettings(PFFServiceSettings PFFServiceSettings)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            PFFServiceMiddleware.AddPFFServices(services, PFFServiceSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<PFFServiceSettings>();

            // Assert
            Assert.Equal(PFFServiceSettings, result);
        }

        [Theory, AutoData]
        public void AddPFFServices_Always_AddIPFFService(PFFServiceSettings PFFServiceSettings)
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "AppSettings:MongoDBSettings:ConnectionString", "mongodb://127.0.0.1:27017" },
                    { "AppSettings:AdminApiSettings:Endpoints:BaseUrl", "https://api.example.com" },
                    { "AppSettings:PFFServiceSettings:Proxy", "http://proxy.example.com" }
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
                PFFServiceSettings = PFFServiceSettings
            };

            services.AddBackendAuthService(appSettings.BackendAuthConfigs);
            services.AddServices();
            services.AddMongoDBServices(appSettings);
            services.AddSingleton<ISentryClient, MockSentryClient>();
            services.AddAdminApiServices(appSettings.AdminApiSettings);
            services.AddSingleton<IAppSettings>(appSettings);

            // Act
            PFFServiceMiddleware.AddPFFServices(services, PFFServiceSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IPFFService>();

            // Assert
            Assert.NotNull(result);
        }
    }
}