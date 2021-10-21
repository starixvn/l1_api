using System.Threading.Tasks;
using L1.Features.Sites;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace L1.Features.SigningCredentials
{
    public class SigningCredentialsDataAccessTests
    {
        private readonly IMongoCollection<SigningCredentialKey> collection;
        private readonly SigningCredentialsDataAccess dataAccess;

        public SigningCredentialsDataAccessTests()
        {
            collection = Substitute.For<IMongoCollection<SigningCredentialKey>>();
            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase("member").Returns(database);
            database.GetCollection<SigningCredentialKey>("signing_credential_keys").Returns(collection);

            dataAccess = new SigningCredentialsDataAccess(mongoClient);
        }

        [Fact]
        public async Task GetMainKeyAsync_Always_ReturnCorrectValue()
        {
            // Act
            var actualValue = await dataAccess.GetMainKeyAsync();

            // Assert
            Assert.Null(actualValue);
        }

        [Fact]
        public async Task GetAllAsync_Always_ReturnAllData()
        {
            // Act
            var actualValue = await dataAccess.GetAllAsync();

            // Assert
            Assert.Empty(actualValue);
        }
    }
}