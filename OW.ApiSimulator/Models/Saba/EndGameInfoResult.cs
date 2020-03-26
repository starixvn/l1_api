using System;

namespace OW.ApiSimulator.Models.Saba
{
    public class EndGameInfoResult : SabaResult
    {
        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public decimal ValidBetAmount { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}