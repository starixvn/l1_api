using System.Threading.Tasks;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.PartnerCommunicators.Log
{
    public interface ILogDataAccess
    {
        Task WriteFailedRequestAsync(PartnerLog log);

        Task WriteBetAsync(PartnerLog log);

        Task WriteEndGameAsync(PartnerLog log);

        Task WriteVoidGameAsync(PartnerLog log);

        Task WriteHitJackpotAsync(PartnerLog log);

        Task WriteSignInAsync(SBLog log);

        Task WriteRetrieveEndGameAsync(SBLog log);

        Task WriteEnterPortalAsync(PartnerLog log);

        Task WriteGetBetLimitAsync(PartnerLog log);

        Task WriteAwardAsync(PartnerLog log);
    }

    public class LogDataAccess : ILogDataAccess
    {
        private const string FailedRequestsCollectionName = "failed_requests";
        private const string BetsCollectionName = "bets";
        private const string EndGamesCollectionName = "end_games";
        private const string VoidGamesCollectionName = "void_games";
        private const string HitJackpotCollectionName = "hit_jackpots";
        private const string SignInCollectionName = "signin_requests";
        private const string RetrieveEndGameCollectionName = "retrieve_requests";
        private const string EnterPortalCollectionName = "enter_portals";
        private const string GetBetLimitCollectionName = "bet_limits";
        private const string AwardCollectionName = "awards";

        private readonly IMongoCollection<PartnerLog> failedRequestsCollection;
        private readonly IMongoCollection<PartnerLog> betsCollection;
        private readonly IMongoCollection<PartnerLog> endGamesCollection;
        private readonly IMongoCollection<PartnerLog> voidGamesCollection;
        private readonly IMongoCollection<PartnerLog> hitJackpotsCollection;
        private readonly IMongoCollection<SBLog> signInCollection;
        private readonly IMongoCollection<SBLog> retrieveCollection;

        private readonly IMongoCollection<PartnerLog> enterPortalsCollection;
        private readonly IMongoCollection<PartnerLog> getBetLimitsCollection;
        private readonly IMongoCollection<PartnerLog> awardCollection;


        public LogDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Log);

            failedRequestsCollection = database.GetCollection<PartnerLog>(FailedRequestsCollectionName);
            betsCollection = database.GetCollection<PartnerLog>(BetsCollectionName);
            endGamesCollection = database.GetCollection<PartnerLog>(EndGamesCollectionName);
            voidGamesCollection = database.GetCollection<PartnerLog>(VoidGamesCollectionName);
            hitJackpotsCollection = database.GetCollection<PartnerLog>(HitJackpotCollectionName);
            signInCollection = database.GetCollection<SBLog>(SignInCollectionName);
            retrieveCollection = database.GetCollection<SBLog>(RetrieveEndGameCollectionName);
            enterPortalsCollection = database.GetCollection<PartnerLog>(EnterPortalCollectionName);
            getBetLimitsCollection = database.GetCollection<PartnerLog>(GetBetLimitCollectionName);
            awardCollection = database.GetCollection<PartnerLog>(AwardCollectionName);
        }

        public Task WriteFailedRequestAsync(PartnerLog log)
            => failedRequestsCollection.InsertOneAsync(log);

        public Task WriteBetAsync(PartnerLog log)
            => betsCollection.InsertOneAsync(log);

        public Task WriteEndGameAsync(PartnerLog log)
            => endGamesCollection.InsertOneAsync(log);

        public Task WriteVoidGameAsync(PartnerLog log)
            => voidGamesCollection.InsertOneAsync(log);

        public Task WriteHitJackpotAsync(PartnerLog log)
           => hitJackpotsCollection.InsertOneAsync(log);

        public Task WriteSignInAsync(SBLog log)
            => signInCollection.InsertOneAsync(log);

        public Task WriteRetrieveEndGameAsync(SBLog log)
            => retrieveCollection.InsertOneAsync(log);

        public Task WriteEnterPortalAsync(PartnerLog log)
            => enterPortalsCollection.InsertOneAsync(log);
        public Task WriteGetBetLimitAsync(PartnerLog log)
            => getBetLimitsCollection.InsertOneAsync(log);

        public Task WriteAwardAsync(PartnerLog log)
            => awardCollection.InsertOneAsync(log);
    }
}