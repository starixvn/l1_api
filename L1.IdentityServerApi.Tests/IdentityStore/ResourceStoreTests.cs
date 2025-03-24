using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using L1.Features.Sites;
using NSubstitute;
using Xunit;

namespace L1.IdentityServerApi.IdentityStore
{
    public class ResourceStoreTests
    {
        private readonly ISiteDataService siteDataService;
        private readonly ResourceStore store;
        private readonly Fixture fixture = new Fixture();

        public ResourceStoreTests()
        {
            siteDataService = Substitute.For<ISiteDataService>();
            store = new ResourceStore(siteDataService);
        }

        [Theory, AutoData]
        public async Task FindApiResourceAsync_Always_ReturnsCorrectApiResource(string name)
        {
            // Arrange 
            var siteId = "SiteId";
            var site = new Site { SiteId = siteId };
            siteDataService.GetBySiteId(Arg.Any<string>()).Returns(site);

            // Act
            var actualValue = await store.FindApiResourcesByNameAsync(name);

            // Assert
            Assert.True(actualValue.Name == siteId && actualValue.DisplayName == siteId);
        }

        [Fact]
        public async Task FindApiResourcesByScopeAsync_Always_ReturnsCorrectApiResource()
        {
            // Arrange 
            var sites = new List<Site>{
                new() { SiteId = "scope1" },
                new() { SiteId = "scope2" },
                new() { SiteId = "scope1" },
            };
            siteDataService.GetAll().Returns(sites);
            var scopeNames = new List<string> { "scope1" };

            // Act
            var actualValue = await store.FindApiResourcesByScopeNameAsync(scopeNames);

            // Assert
            Assert.True(actualValue.Count() == 2);
        }

        [Theory, AutoData]
        public async Task FindIdentityResourcesByScopeAsync_Always_ReturnsEmptyList(IEnumerable<string> scopeNames)
        {
            // Act
            var actualValue = await store.FindIdentityResourcesByScopeNameAsync(scopeNames);

            // Assert
            Assert.Empty(actualValue);
        }

        [Fact]
        public async Task GetAllResourcesAsync_Always_ReturnsCorrectValue()
        {
            // Arrange 
            var sites = fixture.CreateMany<Site>(3);
            siteDataService.GetAll().Returns(sites);

            // Act
            var actualValue = await store.GetAllResourcesAsync();

            // Assert
            Assert.True(actualValue.IdentityResources.Count == 0 && actualValue.ApiResources.Count == 3);
        }
    }
}