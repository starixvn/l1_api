using System.Threading.Tasks;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.Partners
{
    public interface IPartnerDataAccess
    {
        Task<Partner> GetById(string id);

        Task<bool> Verify(string partnerId, string secretKey);
    }

    public class PartnerDataAccess : IPartnerDataAccess
    {
        private const string collectionName = "partners";
        private readonly IMongoCollection<Partner> partnerCollection;

        public PartnerDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Minigame);
            partnerCollection = database.GetCollection<Partner>(collectionName);
        }

        public async Task<Partner> GetById(string id) =>
            (await partnerCollection.FindAsync(site => site.PartnerId == id)).FirstOrDefault();

        public async Task<bool> Verify(string partnerId, string secretKey)
        => (await partnerCollection.CountDocumentsAsync(site => site.PartnerId == partnerId && site.SecretKey == secretKey)) > 0;
    }
}