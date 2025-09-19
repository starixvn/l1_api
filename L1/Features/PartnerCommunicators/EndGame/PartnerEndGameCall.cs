using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace L1.Features.PartnerCommunicators.EndGame
{
    public class PartnerEndGameCall : PartnerCall
    {
#pragma warning disable S107 // Methods should not have too many parameters

        public PartnerEndGameCall(
            object playerId,
            string siteId,
            long gameRoundId,
            int gameTypeId,
            decimal totalAmount,
            decimal totalWin,
            decimal validBetAmount,
            DateTimeOffset endTime,
            bool freeBet,
            object metaData,
            string promotionId,
            string siteName,
            decimal baseTotalAmount)
#pragma warning restore S107 // Methods should not have too many parameters
            : base(playerId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            TotalAmount = totalAmount;
            TotalWin = totalWin;
            ValidBetAmount = validBetAmount;
            EndTime = endTime;
            FreeBet = freeBet;
            MetaData = metaData;
            PromotionId = promotionId;
            SiteName = siteName;
            BaseTotalAmount = baseTotalAmount;
        }

        public long GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public decimal ValidBetAmount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? EndTime { get; set; }

        public bool FreeBet { get; set; }

        public object MetaData { get; set; }
        
        public string PromotionId { get; set; }

        public string SiteName { get; set; }

        [JsonIgnore]
        public decimal BaseTotalAmount { get; set; }
    }
}