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
        public async Task FindClientByIdAsync_ValidSite_ReturnCorrectClient(string clientId)
        {
            // Arrange 
            var site = new Site
            {
                ClientId = clientId,
                SiteId = "Siteid"
            };

            siteDataService.GetByClientId(Arg.Any<string>()).Returns(site);

            // Act
            var actualValue = await store.FindClientByIdAsync(clientId);

            // Assert
            Assert.Equal(actualValue.AllowedGrantTypes, GrantTypes.ClientCredentials);
        }
    }
}