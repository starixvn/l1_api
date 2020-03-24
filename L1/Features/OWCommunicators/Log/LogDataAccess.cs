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
    }

    public class LogDataAccess : ILogDataAccess
    {
        private const string FailedRequestsCollectionName = "failed_requests";
        private const string BetsCollectionName = "bets";
        private const string EndGamesCollectionName = "end_games";

        private readonly IMongoCollection<OWLog> failedRequestsCollection;
        private readonly IMongoCollection<OWLog> betsCollection;
        private readonly IMongoCollection<OWLog> endGamesCollection;

        public LogDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Log);

            failedRequestsCollection = database.GetCollection<OWLog>(FailedRequestsCollectionName);
            betsCollection = database.GetCollection<OWLog>(BetsCollectionName);
            endGamesCollection = database.GetCollection<OWLog>(EndGamesCollectionName);
        }

        public Task WriteFailedRequestAsync(OWLog log)
            => failedRequestsCollection.InsertOneAsync(log);

        public Task WriteBetAsync(OWLog log)
            => betsCollection.InsertOneAsync(log);

        public Task WriteEndGameAsync(OWLog log)
            => endGamesCollection.InsertOneAsync(log);
    }
}