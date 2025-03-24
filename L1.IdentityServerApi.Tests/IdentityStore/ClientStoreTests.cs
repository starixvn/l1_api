using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using IdentityServer4.Models;
using L1.Features.Sites;
using L1.IdentityServerApi.IdentityStore;
using NSubstitute;
using Xunit;

namespace L1.IdentityServerApi.Tests.IdentityStore
{
    public class ClientStoreTests
    {
        private readonly ISiteDataService siteDataService;
        private readonly ClientStore store;

        public ClientStoreTests()
        {
            siteDataService = Substitute.For<ISiteDataService>();
            store = new ClientStore(siteDataService);
        }

        [Theory, AutoData]
        public async Task FindClientByIdAsync_InvalidSite_ReturnDefaultClient(string clientId)
        {
            // Arrange 
            siteDataService.GetByClientId(Arg.Any<string>()).Returns((Site)null);

            // Act
            var actualValue = await store.FindClientByIdAsync(clientId);

            // Assert
            Assert.Null(actualValue);
        }

        [Theory, AutoData]
        public async Task FindClientByIdAsync_ValidSite_ReturnsClientWithCorrectClientId(string clientId)
        {
            // Arrange 
            var site = new Site
            {
                ClientId = clientId,
                SiteId = "SiteId",
                ClientSecret = "secret"
            };

            siteDataService.GetByClientId(Arg.Any<string>()).Returns(site);

            // Act
            var actualValue = await store.FindClientByIdAsync(clientId);

            // Assert
            Assert.Equal(clientId, actualValue.ClientId);
        }

        [Theory, AutoData]
        public async Task FindClientByIdAsync_ValidSite_ReturnsClientWithCorrectGrantTypes(string clientId)
        {
            // Arrange 
            var site = new Site
            {
                ClientId = clientId,
                SiteId = "SiteId",
                ClientSecret = "secret"
            };

            siteDataService.GetByClientId(Arg.Any<string>()).Returns(site);

            // Act
            var actualValue = await store.FindClientByIdAsync(clientId);

            // Assert
            Assert.Equal(GrantTypes.ClientCredentials, actualValue.AllowedGrantTypes);
        }

        [Theory, AutoData]
        public async Task FindClientByIdAsync_ValidSite_ReturnsClientWithCorrectClientSecret(string clientId)
        {
            // Arrange 
            var site = new Site
            {
                ClientId = clientId,
                SiteId = "SiteId",
                ClientSecret = "secret"
            };

            siteDataService.GetByClientId(Arg.Any<string>()).Returns(site);

            // Act
            var actualValue = await store.FindClientByIdAsync(clientId);

            // Assert
            Assert.Single(actualValue.ClientSecrets);
            Assert.Equal(site.ClientSecret.Sha256(), actualValue.ClientSecrets.First().Value);
        }

        [Theory, AutoData]
        public async Task FindClientByIdAsync_ValidSite_ReturnsClientWithCorrectAllowedScopes(string clientId)
        {
            // Arrange 
            var site = new Site
            {
                ClientId = clientId,
                SiteId = "SiteId",
                ClientSecret = "secret"
            };

            siteDataService.GetByClientId(Arg.Any<string>()).Returns(site);

            // Act
            var actualValue = await store.FindClientByIdAsync(clientId);

            // Assert
            Assert.Single(actualValue.AllowedScopes);
            Assert.Equal(site.SiteId, actualValue.AllowedScopes.First());
        }
    }
}