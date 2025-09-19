using System;
using System.Collections.Generic;

namespace L1.WebApi.Models
{
    public class PlaceBet : MemberRequest
    {
        public long GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public int ChoiceId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Ip { get; set; }

        public DateTimeOffset? RoundEndTime { get; set; }

        public bool FreeSpin { get; set; }

        public byte System { get; set; }

        public bool FreeBet { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        public float ContributionRate { get; set; }

        public decimal ContributionAmount { get; set; }

        public string OperatorToken { get; set; }

        public string UserId { get; set; }

        public string PromotionId { get; set; }
    }
}