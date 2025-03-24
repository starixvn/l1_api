using AutoFixture.Xunit2;
using L1.Features.BackendAuthentications;
using L1.Shared.Middlewares;
using L1.Tests._Shared;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Xunit;

namespace L1.Features.AdminApiCommunicators
{
    public class AdminApiMiddlewareTests
    {
        [Theory, AutoData]
        public void AddAdminApiServices_Always_AddEndPointsSingleton(AdminApiSettings adminApiSettings)
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            AdminApiMiddleware.AddAdminApiServices(services, adminApiSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<AdminApiEndPoints>();

            // Assert
            Assert.Equal(adminApiSettings.Endpoints, result);
        }

        [Theory, AutoData]
        public void AddAdminApiServices_Always_AddAdminApiServiceInjection(AdminApiSettings adminApiSettings)
        {
            // Arrange
            var services = new ServiceCollection();
            var backendAuthConfigs = new BackendAuthConfigs();
            services.AddBackendAuthService(backendAuthConfigs);
            services.AddServices();
            services.AddSingleton<ISentryClient, MockSentryClient>();

            services.AddTransient<ProtectedApiBearerTokenHandler>();
            // Act
            AdminApiMiddleware.AddAdminApiServices(services, adminApiSettings);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IAdminApiService>();

            // Assert
            Assert.NotNull(result);
        }
    }
}