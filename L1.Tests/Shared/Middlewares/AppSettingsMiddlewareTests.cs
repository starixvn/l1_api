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
            var configuration = Substitute.For<IConfiguration>();

            // Act
            AppSettingsMiddleware.AddSettings(services, configuration);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IAppSettings>();

            // Assert
            Assert.NotNull(result);
        }
    }
}