using System.Threading.Tasks;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.OWCommunicators.Log
{
    public interface ILogDataAccess
    {
        Task WriteAsync(OWLog log);
    }

    public class LogDataAccess : ILogDataAccess
    {
        private const string CollectionName = "failed_requests";

        private readonly IMongoCollection<OWLog> logCollection;

        public LogDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Log);

            logCollection = database.GetCollection<OWLog>(CollectionName);
        }

        public Task WriteAsync(OWLog log)
            => logCollection.InsertOneAsync(log);
    }
}