namespace L1.Features.OWCommunicators.EndGame
{
    public class EndGameCall : OWCall
    {
        public EndGameCall(int obCustId, string siteId, int gameRoundId, int gameTypeId, decimal totalAmount, decimal totalWin, int validBetAmount) : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            TotalAmount = totalAmount;
            TotalWin = totalWin;
            ValidBetAmount = validBetAmount;
        }

        public int GameRoundId { get; }

        public int GameTypeId { get; }

        public decimal TotalAmount { get; }

        public decimal TotalWin { get; }

        public int ValidBetAmount { get; }
    }
}