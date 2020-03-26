using System;

namespace L1.WebApi.Models
{
    public class EndGame : MemberRequest
    {
        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public int ValidBetAmount { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}