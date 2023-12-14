using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.PartnerCommunicators.EndGame
{
    public class PartnerEndGameCall : PartnerCall
    {
#pragma warning disable S107 // Methods should not have too many parameters

        public PartnerEndGameCall(
            string playerId,
            string siteId,
            int gameRoundId,
            byte gameTypeId,
            decimal totalAmount,
            decimal totalWin,
            decimal validBetAmount,
            DateTimeOffset endTime,
            bool freeBet)
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
        }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public decimal ValidBetAmount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? EndTime { get; set; }

        public bool FreeBet { get; set; }
    }
}