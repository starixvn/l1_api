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

        Task WriteSignInAsync(SBLog log);

        Task WriteRetrieveEndGameAsync(SBLog log);
    }

    public class LogDataAccess : ILogDataAccess
    {
        private const string FailedRequestsCollectionName = "failed_requests";
        private const string BetsCollectionName = "bets";
        private const string EndGamesCollectionName = "end_games";
        private const string VoidGamesCollectionName = "void_games";
        private const string SignInCollectionName = "signin_requests";
        private const string RetrieveEndGameCollectionName = "retrieve_requests";

        private readonly IMongoCollection<OWLog> failedRequestsCollection;
        private readonly IMongoCollection<OWLog> betsCollection;
        private readonly IMongoCollection<OWLog> endGamesCollection;
        private readonly IMongoCollection<OWLog> voidGamesCollection;
        private readonly IMongoCollection<SBLog> signInCollection;
        private readonly IMongoCollection<SBLog> retrieveCollection;

        public LogDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Log);

            failedRequestsCollection = database.GetCollection<OWLog>(FailedRequestsCollectionName);
            betsCollection = database.GetCollection<OWLog>(BetsCollectionName);
            endGamesCollection = database.GetCollection<OWLog>(EndGamesCollectionName);
            voidGamesCollection = database.GetCollection<OWLog>(VoidGamesCollectionName);
            signInCollection = database.GetCollection<SBLog>(SignInCollectionName);
            retrieveCollection = database.GetCollection<SBLog>(RetrieveEndGameCollectionName);
        }

        public Task WriteFailedRequestAsync(OWLog log)
            => failedRequestsCollection.InsertOneAsync(log);

        public Task WriteBetAsync(OWLog log)
            => betsCollection.InsertOneAsync(log);

        public Task WriteEndGameAsync(OWLog log)
            => endGamesCollection.InsertOneAsync(log);

        public Task WriteVoidGameAsync(OWLog log)
            => voidGamesCollection.InsertOneAsync(log);

        public Task WriteSignInAsync(SBLog log)
            => signInCollection.InsertOneAsync(log);

        public Task WriteRetrieveEndGameAsync(SBLog log)
            => retrieveCollection.InsertOneAsync(log);
    }
}