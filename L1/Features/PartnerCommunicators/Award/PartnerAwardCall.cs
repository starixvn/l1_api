using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace L1.Features.PartnerCommunicators.Award
{
    public class PartnerAwardCall : PartnerCall
    {
        public PartnerAwardCall(
            string playerId,
            string siteId,
            int gameTypeId,
            decimal amount,
            int promotionId,
            string currency) : base(playerId, siteId)
        {
            GameTypeId = gameTypeId;
            Amount = amount;
            PromotionId = promotionId;
            Currency = currency;
        }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Amount { get; set; }

        public int GameTypeId { get; set; }

        public int PromotionId { get; set; }

        public string Currency { get; set; }
    }
}