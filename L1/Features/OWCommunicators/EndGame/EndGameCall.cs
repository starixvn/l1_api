namespace L1.Features.OWCommunicators.EndGame
{
    public class EndGameCall : OWCall
    {
        public EndGameCall(string seq, int obCustId, string siteId, int gameRoundId, int gameTypeId, double totalAmount, double totalWin, int validBetAmount) : base(seq, obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            TotalAmount = totalAmount;
            TotalWin = totalWin;
            ValidBetAmount = validBetAmount;
        }

        public int GameRoundId { get; }

        public int GameTypeId { get; }

        public double TotalAmount { get; }

        public double TotalWin { get; }

        public int ValidBetAmount { get; }
    }
}