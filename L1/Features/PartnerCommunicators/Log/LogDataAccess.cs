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
        Task WriteHitJackpotAsync(OWLog log);
        Task WriteSignInAsync(SBLog log);

        Task WriteRetrieveEndGameAsync(SBLog log);
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

        private readonly IMongoCollection<PartnerLog> failedRequestsCollection;
        private readonly IMongoCollection<PartnerLog> betsCollection;
        private readonly IMongoCollection<PartnerLog> endGamesCollection;
        private readonly IMongoCollection<PartnerLog> voidGamesCollection;
        private readonly IMongoCollection<OWLog> hitJackpotsCollection;
        private readonly IMongoCollection<SBLog> signInCollection;
        private readonly IMongoCollection<SBLog> retrieveCollection;

        public LogDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Log);

            failedRequestsCollection = database.GetCollection<PartnerLog>(FailedRequestsCollectionName);
            betsCollection = database.GetCollection<PartnerLog>(BetsCollectionName);
            endGamesCollection = database.GetCollection<PartnerLog>(EndGamesCollectionName);
            voidGamesCollection = database.GetCollection<PartnerLog>(VoidGamesCollectionName);
            hitJackpotsCollection = database.GetCollection<OWLog>(HitJackpotCollectionName);
            signInCollection = database.GetCollection<SBLog>(SignInCollectionName);
            retrieveCollection = database.GetCollection<SBLog>(RetrieveEndGameCollectionName);
        }

        public Task WriteFailedRequestAsync(PartnerLog log)
            => failedRequestsCollection.InsertOneAsync(log);

        public Task WriteBetAsync(PartnerLog log)
            => betsCollection.InsertOneAsync(log);

        public Task WriteEndGameAsync(PartnerLog log)
            => endGamesCollection.InsertOneAsync(log);

        public Task WriteVoidGameAsync(PartnerLog log)
            => voidGamesCollection.InsertOneAsync(log);

         public Task WriteHitJackpotAsync(OWLog log)
            => hitJackpotsCollection.InsertOneAsync(log);
            
        public Task WriteSignInAsync(SBLog log)       
            => signInCollection.InsertOneAsync(log);

        public Task WriteRetrieveEndGameAsync(SBLog log)
            => retrieveCollection.InsertOneAsync(log);
    }
}