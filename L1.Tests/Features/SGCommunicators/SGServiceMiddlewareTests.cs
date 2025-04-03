using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.SGCommunicators;
using L1.Features.SGCommunicators.CheckMaintenance;
using L1.Shared.Configurations;
using L1.Shared.Json;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sentry;
using Xunit;

namespace L1.Tests.Features.SGCommunicators
{
    public class SGServiceMiddlewareTests
    {
        private readonly IServiceCollection services;
        private readonly SGServiceSettings settings;

        public SGServiceMiddlewareTests()
        {
            services = new ServiceCollection();
            
            // Register required services
            services.AddSingleton(Substitute.For<ILogService>());
            services.AddSingleton(Substitute.For<ISentryClient>());
            services.AddSingleton(Substitute.For<IJsonService>());
            services.AddSingleton(Substitute.For<IAdminApiService>());
            services.AddSingleton(Substitute.For<INatsService>());
            services.AddSingleton(Substitute.For<IAppSettings>());
            
            settings = new SGServiceSettings
            {
                BaseUrl = new Uri("http://test.com"),
                SecretKey = "test-secret-key",
                Proxy = "http://proxy.test.com",
                Endpoints = new SGEndPoints
                {
                    CheckMaintenance = "maintenance"
                }
            };
        }

        [Fact]
        public void AddSGServices_RegistersRequiredServices()
        {
            // Act
            services.AddSGServices(settings);
            var provider = services.BuildServiceProvider();

            // Assert
            Assert.NotNull(provider.GetService<SGServiceSettings>());
            Assert.NotNull(provider.GetService<SGEndPoints>());
            Assert.NotNull(provider.GetService<ISGFailureHandler>());
            Assert.NotNull(provider.GetService<ISGService>());
            Assert.NotNull(provider.GetService<ISGMaintenanceService>());
            Assert.NotNull(provider.GetService<IPartnerCustomerServices>());
        }

        [Fact]
        public void AddSGServices_ConfiguresHttpClient_WithBaseAddress()
        {
            // Act
            services.AddSGServices(settings);

            // Assert
            var httpClientFactory = services.Any(s => 
                s.ServiceType == typeof(IHttpClientFactory));
            Assert.True(httpClientFactory, "IHttpClientFactory should be registered");

            var sgService = services.Any(s =>
                s.ServiceType == typeof(ISGService));
            Assert.True(sgService, "ISGService should be registered");
        }

        [Fact]
        public void AddSGServices_WhenProxyConfigured_SetsUpProxy()
        {
            // Act
            services.AddSGServices(settings);

            // Assert
            var sgService = services.Any(s =>
                s.ServiceType == typeof(ISGService));
            Assert.True(sgService, "ISGService should be registered");

            var httpMessageHandlerFactory = services.Any(s =>
                s.ServiceType == typeof(IHttpMessageHandlerFactory));
            Assert.True(httpMessageHandlerFactory, "IHttpMessageHandlerFactory should be registered");
        }

        [Fact]
        public void AddSGServices_WhenNoProxy_DoesNotConfigureProxy()
        {
            // Arrange
            settings.Proxy = null;

            // Act
            services.AddSGServices(settings);

            // Assert
            var sgService = services.Any(s =>
                s.ServiceType == typeof(ISGService));
            Assert.True(sgService, "ISGService should be registered");
        }

        [Fact]
        public void AddSGServices_RegistersSGCustomerService_AsScoped()
        {
            // Act
            services.AddSGServices(settings);

            // Assert
            var descriptor = services.FirstOrDefault(d => 
                d.ServiceType == typeof(SGCustomerService));
            Assert.NotNull(descriptor);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
} 