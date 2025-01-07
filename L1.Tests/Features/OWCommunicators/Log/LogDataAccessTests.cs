using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using L1.Features.MemberAuthentications;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using L1.Shared.Models;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.OWCommunicators.Log
{
    public class LogDataAccessTests
    {
        private readonly IMongoCollection<PartnerLog> failedRequestsCollection;
        private readonly IMongoCollection<PartnerLog> betsCollection;
        private readonly IMongoCollection<PartnerLog> endGamesCollection;
        private readonly IMongoCollection<PartnerLog> voidGamesCollection;
        private readonly IMongoCollection<SBLog> signInCollection;
        private readonly IMongoCollection<SBLog> retrieveCollection;

        private readonly LogDataAccess logDataAccess;

        public LogDataAccessTests()
        {
            failedRequestsCollection = Substitute.For<IMongoCollection<PartnerLog>>();
            betsCollection = Substitute.For<IMongoCollection<PartnerLog>>();
            endGamesCollection = Substitute.For<IMongoCollection<PartnerLog>>();
            voidGamesCollection = Substitute.For<IMongoCollection<PartnerLog>>();
            signInCollection = Substitute.For<IMongoCollection<SBLog>>();
            retrieveCollection = Substitute.For<IMongoCollection<SBLog>>();

            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase(Arg.Any<string>()).Returns(database);

            database.GetCollection<PartnerLog>("failed_requests").Returns(failedRequestsCollection);
            database.GetCollection<PartnerLog>("bets").Returns(betsCollection);
            database.GetCollection<PartnerLog>("end_games").Returns(endGamesCollection);
            database.GetCollection<PartnerLog>("void_games").Returns(voidGamesCollection);
            database.GetCollection<SBLog>("signin_requests").Returns(signInCollection);
            database.GetCollection<SBLog>("retrieve_requests").Returns(retrieveCollection);
            var appSettings = Substitute.For<IAppSettings>();
            appSettings.MongoDBSettings = new MongoDBSettings
            {
                ConnectionString = "mongodb://127.0.0.1:27017",
            };

            logDataAccess = new LogDataAccess(mongoClient, appSettings);
        }

        [Theory, AutoData]
        public async Task WriteFailedRequestAsync_Always_InsertOneAsync(string partnerId, OWCall call, OWResult result)
        {
            // Arrange
            var log = new PartnerLog(partnerId, DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result, AppTypes.SabaClub);

            // Act
            await logDataAccess.WriteFailedRequestAsync(log);

            // Assert
            await failedRequestsCollection.Received().InsertOneAsync(Arg.Any<PartnerLog>());
        }

        [Theory, AutoData]
        public async Task WriteBetAsync_Always_InsertOneAsync(string partnerId, OWCall call, OWResult result)
        {
            // Arrange
            var log = new PartnerLog(partnerId, DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result, AppTypes.SabaClub);

            // Act
            await logDataAccess.WriteBetAsync(log);

            // Assert
            await betsCollection.Received().InsertOneAsync(Arg.Any<PartnerLog>());
        }

        [Theory, AutoData]
        public async Task WriteEndGameAsync_Always_InsertOneAsync(string partnerId, OWCall call, OWResult result)
        {
            // Arrange
            var log = new PartnerLog(partnerId, DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result, AppTypes.SabaClub);

            // Act
            await logDataAccess.WriteEndGameAsync(log);

            // Assert
            await endGamesCollection.Received().InsertOneAsync(Arg.Any<PartnerLog>());
        }

        [Theory, AutoData]
        public async Task WriteVoidGameAsync_Always_InsertOneAsync(string partnerId, OWCall call, OWResult result)
        {
            // Arrange
            var log = new PartnerLog(partnerId, DateTimeOffset.UtcNow, call, DateTimeOffset.UtcNow, result, AppTypes.SabaClub);

            // Act
            await logDataAccess.WriteVoidGameAsync(log);

            // Assert
            await voidGamesCollection.Received().InsertOneAsync(Arg.Any<PartnerLog>());
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