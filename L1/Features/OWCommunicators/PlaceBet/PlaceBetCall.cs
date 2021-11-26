using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.OWCommunicators.PlaceBet
{
    public class PlaceBetCall : OWCall
    {
#pragma warning disable S107 // Methods should not have too many parameters
        public PlaceBetCall(
            int obCustId,
            string siteId,
            int gameRoundId,
            byte gameTypeId,
            int choiceId,
            decimal amount,
            string currency,
            DateTimeOffset? roundEndTime,
            string ip,
            bool freeSpin) : base(obCustId, siteId)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            ChoiceId = choiceId;
            Amount = amount;
            Currency = currency;
            RoundEndTime = roundEndTime;
            Ip = ip;
            FreeSpin = freeSpin;
        }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public int ChoiceId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset? RoundEndTime { get; set; }

        public string Ip { get; set; }

        public bool FreeSpin { get; set; }
    }
}