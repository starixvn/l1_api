using System;

namespace L1.Client
{
    public class EndGameInfoResponse
    {
        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public decimal ValidBetAmount { get; set; }

        public string Seq { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}