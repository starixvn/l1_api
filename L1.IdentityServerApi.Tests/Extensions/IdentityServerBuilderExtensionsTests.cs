using IdentityServer4.Stores;
using L1.IdentityServerApi.IdentityStore;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using L1.Features.SigningCredentials;
using L1.Shared.Configurations;

namespace L1.IdentityServerApi.Tests.Extensions
{
    public class IdentityServerBuilderExtensionsTests
    {
        private readonly Microsoft.Extensions.DependencyInjection.IIdentityServerBuilder builder;
        private readonly ServiceCollection services;

        public IdentityServerBuilderExtensionsTests()
        {
            builder = Substitute.For<Microsoft.Extensions.DependencyInjection.IIdentityServerBuilder>();
            services = new ServiceCollection();
            var appSettings = Substitute.For<IAppSettings>();
            appSettings.MongoDBSettings = new MongoDBSettings
            {
                ConnectionString = "mongodb://127.0.0.1:27017",
            };
            services.AddSingleton<IAppSettings>(appSettings);
            builder.Services.Returns(services);
        }

        [Fact]
        public void AddMongoDb_Always_AddISigningCredentialsDataAccessServices()
        {
            // Arrange 
            var connectionString = "mongodb://127.0.0.1:27017";

            // Act
            IdentityServerApi.Extensions.IdentityServerBuilderExtensions.AddMongoDb(builder, connectionString);

            // Assert
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<ISigningCredentialsDataAccess>();
            Assert.NotNull(result);
        }

        [Fact]
        public void AddIdentityClients_Always_AddClientStoreServices()
        {
            // Arrange 
            var connectionString = "mongodb://127.0.0.1:27017";
            IdentityServerApi.Extensions.IdentityServerBuilderExtensions.AddMongoDb(builder, connectionString);

            // Act
            IdentityServerApi.Extensions.IdentityServerBuilderExtensions.AddIdentityClients(builder);

            // Assert
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<ClientStore>();
            Assert.NotNull(result);
        }

        [Fact]
        public void AddIdentityApiResources_Always_AddResourceStoreServices()
        {
            // Arrange 
            var connectionString = "mongodb://127.0.0.1:27017";
            IdentityServerApi.Extensions.IdentityServerBuilderExtensions.AddMongoDb(builder, connectionString);

            // Act
            IdentityServerApi.Extensions.IdentityServerBuilderExtensions.AddIdentityApiResources(builder);

            // Assert
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<ResourceStore>();
            Assert.NotNull(result);
        }

        [Fact]
        public void AddSigningCredentialStore_Always_AddSigningCredentialStoreToServices()
        {
            // Arrange 
            var connectionString = "mongodb://127.0.0.1:27017";
            IdentityServerApi.Extensions.IdentityServerBuilderExtensions.AddMongoDb(builder, connectionString);

            // Act
            IdentityServerApi.Extensions.IdentityServerBuilderExtensions.AddSigningCredentialStore(builder);

            // Assert
            var provider = services.BuildServiceProvider();
            var result = provider.GetRequiredService<ISigningCredentialStore>();
            Assert.NotNull(result);
        }
    }
}