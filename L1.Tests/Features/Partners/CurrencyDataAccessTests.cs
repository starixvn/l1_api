using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.Partners;
using L1.Shared.Configurations;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.Partners
{
    public class CurrencyDataAccessTests
    {
        private readonly IMongoCollection<Currency> currencyCollection;

        private readonly CurrencyDataAccess dataAccess;

        public CurrencyDataAccessTests()
        {
            currencyCollection = Substitute.For<IMongoCollection<Currency>>();

            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase(Arg.Any<string>()).Returns(database);
            database.GetCollection<Currency>("currencies").Returns(currencyCollection);
            var appSettings = Substitute.For<IAppSettings>();
            appSettings.MongoDBSettings = new MongoDBSettings
            {
                ConnectionString = "mongodb://127.0.0.1:27017",
            };

            dataAccess = new CurrencyDataAccess(mongoClient, appSettings);
        }

        [Theory, AutoData]
        public async Task Verify_InvalidCurrency_ReturnApproriateValue(string partnerId, string currencyName)
        {
            // Arrange
            currencyCollection.CountDocumentsAsync(Arg.Any<FilterDefinition<Currency>>()).Returns(0);

            // Act
            var actualValue = await dataAccess.Verify(partnerId, currencyName);

            // Assert
            Assert.False(actualValue);
        }

        [Theory, AutoData]
        public async Task Verify_ValidCurrency_ReturnApproriateValue(string partnerId, string currencyName)
        {
            // Arrange
            currencyCollection.CountDocumentsAsync(Arg.Any<FilterDefinition<Currency>>(), Arg.Any<CountOptions>()).Returns(1);

            // Act
            var actualValue = await dataAccess.Verify(partnerId, currencyName);

            // Assert
            Assert.True(actualValue);
        }
    }
}