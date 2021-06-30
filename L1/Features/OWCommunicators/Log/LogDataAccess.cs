using System.Threading.Tasks;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.OWCommunicators.Log
{
    public interface ILogDataAccess
    {
        Task WriteFailedRequestAsync(OWLog log);

        Task WriteBetAsync(OWLog log);

        Task WriteEndGameAsync(OWLog log);

        Task WriteVoidGameAsync(OWLog log);
        Task WriteHitJackpotAsync(OWLog log);
        Task WriteSignInAsync(SBLog log);
    }

    public class LogDataAccess : ILogDataAccess
    {
        private const string FailedRequestsCollectionName = "failed_requests";
        private const string BetsCollectionName = "bets";
        private const string EndGamesCollectionName = "end_games";
        private const string VoidGamesCollectionName = "void_games";
        private const string HitJackpotCollectionName = "hit_jackpots";
        private const string SignInCollectionName = "signin_requests";

        private readonly IMongoCollection<OWLog> failedRequestsCollection;
        private readonly IMongoCollection<OWLog> betsCollection;
        private readonly IMongoCollection<OWLog> endGamesCollection;
        private readonly IMongoCollection<OWLog> voidGamesCollection;
        private readonly IMongoCollection<OWLog> hitJackpotsCollection;
        private readonly IMongoCollection<SBLog> signInCollection;

        public LogDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Log);

            failedRequestsCollection = database.GetCollection<OWLog>(FailedRequestsCollectionName);
            betsCollection = database.GetCollection<OWLog>(BetsCollectionName);
            endGamesCollection = database.GetCollection<OWLog>(EndGamesCollectionName);
            voidGamesCollection = database.GetCollection<OWLog>(VoidGamesCollectionName);
            hitJackpotsCollection = database.GetCollection<OWLog>(HitJackpotCollectionName);
            signInCollection = database.GetCollection<SBLog>(SignInCollectionName);
        }

        public Task WriteFailedRequestAsync(OWLog log)
            => failedRequestsCollection.InsertOneAsync(log);

        public Task WriteBetAsync(OWLog log)
            => betsCollection.InsertOneAsync(log);

        public Task WriteEndGameAsync(OWLog log)
            => endGamesCollection.InsertOneAsync(log);

        public Task WriteVoidGameAsync(OWLog log)
            => voidGamesCollection.InsertOneAsync(log);

         public Task WriteHitJackpotAsync(OWLog log)
            => hitJackpotsCollection.InsertOneAsync(log);
            
        public Task WriteSignInAsync(SBLog log)       
            => signInCollection.InsertOneAsync(log);
    }
}