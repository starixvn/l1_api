using System.Collections.Generic;
using AutoFixture.Xunit2;
using L1.Features.AdminApiCommunicators;
using L1.Features.BackendAuthentications;
using L1.Shared.Configurations;
using L1.Shared.Middlewares;
using L1.Tests._Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Xunit;

namespace L1.Features.OWCommunicators
{
    public class OWServiceMiddlewareTests
    {
        [Theory, AutoData]
        public void AddOWServices_Always_AddOWServiceSettings(OWServiceSettings owServiceSettings)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            OWServiceMiddleware.AddOWServices(services, owServiceSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<OWServiceSettings>();

            // Assert
            Assert.Equal(owServiceSettings, result);
        }

        [Theory, AutoData]
        public void AddOWServices_Always_AddIOWService(OWServiceSettings owServiceSettings)
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
           .AddInMemoryCollection(new Dictionary<string, string>
           {
                { "AppSettings:MongoDBSettings:ConnectionString", "mongodb://127.0.0.1:27017" },
                { "AppSettings:AdminApiSettings:Endpoints:BaseUrl", "https://api.example.com" },
                { "AppSettings:OWServiceSettings:Proxy", "http://proxy.example.com" }
           })
           .Build();

            services.AddSettings(configuration);
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
                OWServiceSettings = owServiceSettings
            };

            services.AddBackendAuthService(appSettings.BackendAuthConfigs);
            services.AddServices();
            services.AddMongoDBServices(appSettings);
            services.AddSingleton<ISentryClient, MockSentryClient>();
            services.AddAdminApiServices(appSettings.AdminApiSettings);
            services.AddSingleton<IAppSettings>(appSettings);

            // Act
            OWServiceMiddleware.AddOWServices(services, owServiceSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IOWService>();

            // Assert
            Assert.NotNull(result);
        }
    }
}