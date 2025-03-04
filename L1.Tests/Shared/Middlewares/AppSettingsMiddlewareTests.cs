using System.Collections.Generic;
using L1.Shared.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace L1.Shared.Middlewares
{
    public class AppSettingsMiddlewareTests
    {
        [Fact]
        public void AddSettings_Always_AddSingletonIAppSettings()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "AppSettings:MongoDBSettings:ConnectionString", "mongodb://127.0.0.1:27017" },
                { "AppSettings:AdminApiSettings:Endpoints:BaseUrl", "https://api.example.com" }
            })
            .Build();

            // Act
            AppSettingsMiddleware.AddSettings(services, configuration);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IAppSettings>();

            // Assert
            Assert.NotNull(result);
        }
    }
}