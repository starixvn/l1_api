using System;

namespace L1.Features.OWCommunicators.EndGame
{
    public class EndGameCall : OWCall
    {
        public EndGameCall(int obCustId, string siteId, int gameRoundId, int gameTypeId, decimal totalAmount, decimal totalWin, int validBetAmount, DateTimeOffset endTime) : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            TotalAmount = totalAmount;
            TotalWin = totalWin;
            ValidBetAmount = validBetAmount;
            EndTime = endTime;
        }

        public int GameRoundId { get; }

        public int GameTypeId { get; }

        public decimal TotalAmount { get; }

        public decimal TotalWin { get; }

        public int ValidBetAmount { get; }

        public DateTimeOffset EndTime { get; set; }

    }
}