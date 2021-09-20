using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.Log;
using L1.Shared.Models;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.OWCommunicators.Log
{
    public class LogDataAccessTests
    {
        private readonly IMongoCollection<OWLog> failedRequestsCollection;
        private readonly IMongoCollection<OWLog> betsCollection;
        private readonly IMongoCollection<OWLog> endGamesCollection;
        private readonly IMongoCollection<OWLog> voidGamesCollection;
        private readonly IMongoCollection<SBLog> signInCollection;
        private readonly IMongoCollection<SBLog> retrieveCollection;

        private readonly LogDataAccess logDataAccess;

        public LogDataAccessTests()
        {
            failedRequestsCollection = Substitute.For<IMongoCollection<OWLog>>();
            betsCollection = Substitute.For<IMongoCollection<OWLog>>();
            endGamesCollection = Substitute.For<IMongoCollection<OWLog>>();
            voidGamesCollection = Substitute.For<IMongoCollection<OWLog>>();
            signInCollection = Substitute.For<IMongoCollection<SBLog>>();
            retrieveCollection = Substitute.For<IMongoCollection<SBLog>>();

            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase(Arg.Any<string>()).Returns(database);

            database.GetCollection<OWLog>("failed_requests").Returns(failedRequestsCollection);
            database.GetCollection<OWLog>("bets").Returns(betsCollection);
            database.GetCollection<OWLog>("end_games").Returns(endGamesCollection);
            database.GetCollection<OWLog>("void_games").Returns(voidGamesCollection);
            database.GetCollection<SBLog>("signin_requests").Returns(signInCollection);
            database.GetCollection<SBLog>("retrieve_requests").Returns(retrieveCollection);

            logDataAccess = new LogDataAccess(mongoClient);
        }

        [Theory, AutoData]
        public async Task WriteFailedRequestAsync_Always_InsertOneAsync(OWCall call, OWResult result)
        {
            // Arrange
            var log = new OWLog(DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result);

            // Act
            await logDataAccess.WriteFailedRequestAsync(log);

            // Assert
            await failedRequestsCollection.Received().InsertOneAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task WriteBetAsync_Always_InsertOneAsync(OWCall call, OWResult result)
        {
            // Arrange
            var log = new OWLog(DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result);

            // Act
            await logDataAccess.WriteBetAsync(log);

            // Assert
            await betsCollection.Received().InsertOneAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task WriteEndGameAsync_Always_InsertOneAsync(OWCall call, OWResult result)
        {
            // Arrange
            var log = new OWLog(DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result);

            // Act
            await logDataAccess.WriteEndGameAsync(log);

            // Assert
            await endGamesCollection.Received().InsertOneAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task WriteVoidGameAsync_Always_InsertOneAsync(OWCall call, OWResult result)
        {
            // Arrange
            var log = new OWLog(DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result);

            // Act
            await logDataAccess.WriteVoidGameAsync(log);

            // Assert
            await voidGamesCollection.Received().InsertOneAsync(Arg.Any<OWLog>());
        }

        [Theory, AutoData]
        public async Task WriteSignInAsync_Always_InsertOneAsync(Request request, SingleSignOnResult result)
        {
            // Arrange
            var log = new SBLog(request, result, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            await logDataAccess.WriteSignInAsync(log);

            // Assert
            await signInCollection.Received().InsertOneAsync(Arg.Any<SBLog>());
        }

        [Theory, AutoData]
        public async Task WriteRetrieveEndGameAsync_Always_InsertOneAsync(Request request, SingleSignOnResult result)
        {
            // Arrange
            var log = new SBLog(request, result, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            await logDataAccess.WriteRetrieveEndGameAsync(log);

            // Assert
            await retrieveCollection.Received().InsertOneAsync(Arg.Any<SBLog>());
        }
    }
}