using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L1.Features.PartnerCommunicators.PlaceBet
{
    public class PartnerPlaceBetCall : PartnerCall
    {
#pragma warning disable S107 // Methods should not have too many parameters

        public PartnerPlaceBetCall(
            int playerId,
            string siteId,
            int gameRoundId,
            byte gameTypeId,
            int choiceId,
            decimal amount,
            string currency,
            DateTimeOffset? roundEndTime,
            string ip,
            bool freeSpin,
            decimal contributionAmount,
            float contributionRate,
            bool freeBet) : base(playerId, siteId)
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
            ContributionAmount = contributionAmount;
            ContributionRate = contributionRate;
            FreeBet = freeBet;
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

        public decimal ContributionAmount { get; set; }

        public float ContributionRate { get; set; }

        public bool FreeBet { get; set; }
    }
}