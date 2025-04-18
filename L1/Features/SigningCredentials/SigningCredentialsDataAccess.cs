using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Features.Sites;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.SigningCredentials
{
    public interface ISigningCredentialsDataAccess
    {
        Task<SigningCredentialKey> GetMainKeyAsync();
        Task<IEnumerable<SigningCredentialKey>> GetAllAsync();
    }

    public class SigningCredentialsDataAccess : ISigningCredentialsDataAccess
    {
        private const string collectionName = "signing_credential_keys";
        private readonly IMongoCollection<SigningCredentialKey> collection;

        public SigningCredentialsDataAccess(IMongoClient mongoClient, IAppSettings appSettings)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.GetMemberDatabase(appSettings.MongoDBSettings.PrefixDbName));
            collection = database.GetCollection<SigningCredentialKey>(collectionName);
        }

        public async Task<SigningCredentialKey> GetMainKeyAsync()
        {
            return await collection.Find(x => x.IsMain).SortByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SigningCredentialKey>> GetAllAsync()
        {
            return await collection.Find(x => true).ToListAsync();
        }
    }
}