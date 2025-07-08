using System;

namespace L1.WebApi.Models
{
    public class EndGame : MemberRequest
    {
        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public decimal ValidBetAmount { get; set; }

        public bool FreeBet { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public EndGameMetaData MetaData { get; set; }
        
        public string PromotionId { get; set; }

        public string SiteName { get; set; }
    }
}