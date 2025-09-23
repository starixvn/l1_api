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
            string promotionId,
            string currency,
            byte awardType) : base(playerId, siteId)
        {
            GameTypeId = gameTypeId;
            Amount = amount;
            PromotionId = promotionId;
            Currency = currency;
            AwardType = awardType;
        }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Amount { get; set; }

        public int GameTypeId { get; set; }

        public string PromotionId { get; set; }

        public string Currency { get; set; }

        public byte AwardType { get; set; }
    }
}