using System;

namespace OW.ApiSimulator.Models.OW
{
    public class EndGameCall : OWCall
    {
        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public int ValidBetAmount { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}