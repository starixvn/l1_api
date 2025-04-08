using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.Partners;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.Partners
{
    public class PartnerDataAccessTests
    {
        private readonly IMongoCollection<Partner> partnerCollection;

        private readonly PartnerDataAccess dataAccess;

        public PartnerDataAccessTests()
        {
            partnerCollection = Substitute.For<IMongoCollection<Partner>>();

            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase(Arg.Any<string>()).Returns(database);
            database.GetCollection<Partner>("partners").Returns(partnerCollection);

            dataAccess = new PartnerDataAccess(mongoClient);
        }

        [Theory, AutoData]
        public async Task Verify_InvalidPartner_ReturnApproriateValue(string partnerId, string secretKey)
        {
            // Arrange
            partnerCollection.CountDocumentsAsync(Arg.Any<FilterDefinition<Partner>>()).Returns(0);

            // Act
            var actualValue = await dataAccess.Verify(partnerId, secretKey);

            // Assert
            Assert.False(actualValue);
        }

        [Theory, AutoData]
        public async Task Verify_ValidPartner_ReturnApproriateValue(string partnerId, string secretKey)
        {
            // Arrange
            partnerCollection.CountDocumentsAsync(Arg.Any<FilterDefinition<Partner>>()).Returns(1);

            // Act
            var actualValue = await dataAccess.Verify(partnerId, secretKey);

            // Assert
            Assert.True(actualValue);
        }
    }
}