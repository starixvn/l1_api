using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace L1.Features.Sites
{
    public class SiteDataAccessTests
    {
        private readonly IMongoCollection<Site> siteCollection;

        private readonly SiteDataAccess dataAccess;
        private readonly Fixture fixture = new Fixture();

        public SiteDataAccessTests()
        {
            siteCollection = Substitute.For<IMongoCollection<Site>>();

            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase(Arg.Any<string>()).Returns(database);
            database.GetCollection<Site>("sites").Returns(siteCollection);

            dataAccess = new SiteDataAccess(mongoClient);
        }

        [Fact]
        public void GetAll_Always_ReturnAllSites()
        {
            // Act
            var actualValue = dataAccess.GetAll();

            // Assert
            Assert.Empty(actualValue);
        }

        [Theory, AutoData]
        public void GetByClientId_Always_ReturnCorrectSite(string id)
        {
            // Act
            var actualValue = dataAccess.GetByClientId(id);

            // Assert
            Assert.Null(actualValue);
        }

        [Theory, AutoData]
        public void GetBySiteId_Always_ReturnCorrectSite(string id)
        {
            // Act
            var actualValue = dataAccess.GetBySiteId(id);

            // Assert
            Assert.Null(actualValue);
        }

        [Theory, AutoData]
        public void VerifyClient_Always_ReturnCorrectSite(string clientId, string clientSecret)
        {
            // Act
            var actualValue = dataAccess.VerifyClient(clientId, clientSecret);

            // Assert
            Assert.False(actualValue);
        }

        [Theory, AutoData]
        public async Task VerifyClientWithCurrency_Always_ReturnCorrectSite(string clientId, string clientSecret, string currency)
        {
            // Act
            var actualValue = await dataAccess.VerifyClient(clientId, clientSecret, currency);

            // Assert
            Assert.False(actualValue);
        }
    }
}