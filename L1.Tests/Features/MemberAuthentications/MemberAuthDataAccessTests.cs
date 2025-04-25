using System.Threading.Tasks;
using AutoFixture.Xunit2;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace L1.Features.MemberAuthentications
{
    public class MemberAuthDataAccessTests
    {
        private readonly IMongoCollection<MemberAuth> memberAuthCollection;
        private readonly MemberAuthDataAccess dataAccess;

        public MemberAuthDataAccessTests()
        {
            memberAuthCollection = Substitute.For<IMongoCollection<MemberAuth>>();
            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase("member").Returns(database);
            database.GetCollection<MemberAuth>("member_auths").Returns(memberAuthCollection);

            dataAccess = new MemberAuthDataAccess(mongoClient);
        }

        [Theory, AutoData]
        public async Task GetByToken_Always_ReturnCorrectMemberAuth(string token)
        {
            // Act
            var actualValue = await dataAccess.GetByToken(token);

            // Assert
            Assert.Null(actualValue);
        }
    }
}