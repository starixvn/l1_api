using System;
using System.Collections.Generic;
using L1.Features.BackendAuthentications;
using L1.Features.Sites;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace L1.IdentityServerApi.Tests
{
    public class StartupTests
    {
        private readonly IConfiguration configuration;
        private readonly Startup startup;

        public StartupTests()
        {
            configuration = Substitute.For<IConfiguration>();
            var configurationSection = Substitute.For<IConfigurationSection>();
            var children = new List<IConfigurationSection> { Substitute.For<IConfigurationSection>() };
            configurationSection.GetChildren().Returns(children);

            var tokenExpiredSecondsSection = Substitute.For<IConfigurationSection>();
            tokenExpiredSecondsSection.Value = "60";
            configurationSection.GetSection("TokenExpiredSeconds").Returns(tokenExpiredSecondsSection);

            var highStakeSection = Substitute.For<IConfigurationSection>();
            highStakeSection.Value = "60";
            configurationSection.GetSection("HighStake").Returns(highStakeSection);

            AddBackendAuthConfigsSection(configurationSection);
            AddMongoDBSettingsConfigsSection(configurationSection);
            AddOWServiceSettingsConfigsSection(configurationSection);
            AddAdminApiSettingsConfigsSection(configurationSection);

            configuration.GetSection("AppSettings").Returns(configurationSection);
            startup = new Startup(configuration);
        }

        [Fact]
        public void ConfigureServices_Always_RegisterCorrectServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            startup.ConfigureServices(services);

            // Assert
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<ISiteDataAccess>();
            Assert.NotNull(result);
        }

        [Fact]
        public void Configure_IsDevelopment_HaveErrorWhenTest()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddRouting();
            var provider = services.BuildServiceProvider();
            var builder = new ApplicationBuilder(provider);

            var env = Substitute.For<IWebHostEnvironment>();
            env.EnvironmentName.Returns(Environments.Development);

            startup.ConfigureServices(services);

            // Assert
            Assert.ThrowsAny<Exception>(() => startup.Configure(builder, env));
        }

        [Fact]
        public void Configure_NotIsDevelopment_HaveErrorWhenTest()
        {
            // Arrange
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();
            var app = Substitute.For<IApplicationBuilder>();
            app.ApplicationServices = provider;

            var env = Substitute.For<IWebHostEnvironment>();
            env.EnvironmentName.Returns(Environments.Production);

            startup.ConfigureServices(services);

            // Assert
            Assert.ThrowsAny<Exception>(() => startup.Configure(app, env));
        }

        private void AddBackendAuthConfigsSection(IConfigurationSection configSection)
        {
            var children = new List<IConfigurationSection> { Substitute.For<IConfigurationSection>() };
            var backendAuthConfigsSection = Substitute.For<IConfigurationSection>();
            backendAuthConfigsSection.GetChildren().Returns(children);
            configSection.GetSection("BackendAuthConfigs").Returns(backendAuthConfigsSection);

            var address = Substitute.For<IConfigurationSection>();
            address.Value = "Address";
            backendAuthConfigsSection.GetSection("Address").Returns(address);
        }

        private void AddMongoDBSettingsConfigsSection(IConfigurationSection configSection)
        {
            var children = new List<IConfigurationSection> { Substitute.For<IConfigurationSection>() };
            var mongoDBSettingsConfigsSection = Substitute.For<IConfigurationSection>();
            mongoDBSettingsConfigsSection.GetChildren().Returns(children);
            configSection.GetSection("MongoDBSettings").Returns(mongoDBSettingsConfigsSection);

            var connectionString = Substitute.For<IConfigurationSection>();
            connectionString.Value = "mongodb://127.0.0.1:27017";
            mongoDBSettingsConfigsSection.GetSection("ConnectionString").Returns(connectionString);
        }

        private void AddOWServiceSettingsConfigsSection(IConfigurationSection configSection)
        {
            var children = new List<IConfigurationSection> { Substitute.For<IConfigurationSection>() };
            var owServiceSettingsConfigsSection = Substitute.For<IConfigurationSection>();
            owServiceSettingsConfigsSection.GetChildren().Returns(children);
            configSection.GetSection("OWServiceSettings").Returns(owServiceSettingsConfigsSection);

            var owEndPoints = Substitute.For<IConfigurationSection>();
            owEndPoints.GetChildren().Returns(children);
            owServiceSettingsConfigsSection.GetSection("Endpoints").Returns(owEndPoints);
        }

        private void AddAdminApiSettingsConfigsSection(IConfigurationSection configSection)
        {
            var children = new List<IConfigurationSection> { Substitute.For<IConfigurationSection>() };
            var adminApiSettingsConfigsSection = Substitute.For<IConfigurationSection>();
            adminApiSettingsConfigsSection.GetChildren().Returns(children);
            configSection.GetSection("AdminApiSettings").Returns(adminApiSettingsConfigsSection);

            var owEndPoints = Substitute.For<IConfigurationSection>();
            owEndPoints.GetChildren().Returns(children);
            adminApiSettingsConfigsSection.GetSection("Endpoints").Returns(owEndPoints);
        }
    }
}