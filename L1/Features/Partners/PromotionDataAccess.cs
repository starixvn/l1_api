using System.Threading.Tasks;
using L1.Shared.Constants;
using MongoDB.Driver;

namespace L1.Features.Partners
{
    public interface IPromotionDataAccess
    {
        Task<Promotion> GetPromotion(string partnerId, string promotionCode);
    }

    public class PromotionDataAccess : IPromotionDataAccess
    {
        private readonly IMongoCollection<Promotion> promotionCollection;

        public PromotionDataAccess(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(MongoDatabases.Minigame);
            promotionCollection = database.GetCollection<Promotion>("game_promotions");
        }

        public async Task<Promotion> GetPromotion(string partnerId, string promotionCode)
        {
            var filter = Builders<Promotion>.Filter.And(
                Builders<Promotion>.Filter.Eq(x => x.PartnerId, partnerId),
                Builders<Promotion>.Filter.Eq(x => x.PromotionCode, promotionCode));

            return (await promotionCollection.FindAsync(filter)).FirstOrDefault();
        }
    }
} 