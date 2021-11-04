using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using IdentityModel.Client;
using L1.Tests._Shared;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace L1.Features.BackendAuthentications
{
    public class BackendAuthMiddlewareTests
    {
        [Theory, AutoData]
        public async Task AddOWServices_Always_AddOWServiceSettings(BackendAuthConfigs backendAuthConfigs)
        {
            // Arrange
            var services = new ServiceCollection();
            backendAuthConfigs.Address = "https://localhost";

            // Act
            BackendAuthMiddleware.AddBackendAuthService(services, backendAuthConfigs);
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<IDiscoveryCache>();
            await result.GetAsync();

            // Assert
            Assert.NotNull(result);
        }
    }
}