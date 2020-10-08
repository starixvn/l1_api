using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.OWCommunicators.EndGame
{
    public class EndGameCall : OWCall
    {
        public EndGameCall(
            int obCustId, 
            string siteId, 
            int gameRoundId, 
            byte gameTypeId, 
            decimal totalAmount, 
            decimal totalWin, 
            int validBetAmount, 
            DateTimeOffset endTime,
            bool freeBet) : base(obCustId, siteId)
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

        public int ValidBetAmount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? EndTime { get; set; }

        public bool FreeBet { get; set; }
    }
}