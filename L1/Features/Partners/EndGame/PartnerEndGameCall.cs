using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.Partners.EndGame
{
    public class PartnerEndGameCall : PartnerCall
    {
#pragma warning disable S107 // Methods should not have too many parameters

        public PartnerEndGameCall(
            int obCustId,
            string siteId,
            int gameRoundId,
            byte gameTypeId,
            decimal totalAmount,
            decimal totalWin,
            decimal validBetAmount,
            DateTimeOffset endTime)
#pragma warning restore S107 // Methods should not have too many parameters
            : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            TotalAmount = totalAmount;
            TotalWin = totalWin;
            ValidBetAmount = validBetAmount;
            EndTime = endTime;
        }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public decimal ValidBetAmount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? EndTime { get; set; }
    }
}