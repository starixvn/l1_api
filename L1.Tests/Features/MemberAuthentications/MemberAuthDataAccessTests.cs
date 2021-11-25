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

        [Fact]
        public void GetAll_Always_ReturnAllMemberAuthCollection()
        {
            // Act
            var actualValue = dataAccess.GetAll();

            // Assert
            Assert.Empty(actualValue);
        }

        [Theory, AutoData]
        public void Get_Always_ReturnCorrectMemberAuth(string id)
        {
            // Act
            var actualValue = dataAccess.Get(id);

            // Assert
            Assert.Null(actualValue);
        }

        [Theory, AutoData]
        public void GetByToken_Always_ReturnCorrectMemberAuth(string token)
        {
            // Act
            var actualValue = dataAccess.GetByToken(token);

            // Assert
            Assert.Null(actualValue);
        }

        [Theory, AutoData]
        public void GetLatest_Always_ReturnCorrectMemberAuth(MemberAuth memberAuth)
        {
            // Act
            var actualValue = dataAccess.GetLatest(memberAuth);

            // Assert
            Assert.Null(actualValue);
        }

        [Theory, AutoData]
        public void Create_Always_ReturnCorrectMemberAuth(MemberAuth memberAuth)
        {
            // Act
            dataAccess.Create(memberAuth);

            // Assert
            memberAuthCollection.Received().InsertOne(memberAuth);
        }

        [Theory, AutoData]
        public void Update_Always_ReturnCorrectMemberAuth(string id, MemberAuth memberAuth)
        {
            // Act
            dataAccess.Update(id, memberAuth);

            // Assert
            memberAuthCollection.Received().ReplaceOne(Arg.Any<FilterDefinition<MemberAuth>>(), Arg.Any<MemberAuth>());
        }
    }
}