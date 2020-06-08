using System;

namespace L1.WebApi.Models
{
    public class PlaceBet : MemberRequest
    {
        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public int ChoiceId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Ip { get; set; }

        public DateTimeOffset? RoundEndTime { get; set; }
    }
}