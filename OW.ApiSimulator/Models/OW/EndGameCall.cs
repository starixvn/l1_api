namespace OW.ApiSimulator.Models.OW
{
    public class EndGameCall : OWCall
    {
        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public double TotalAmount { get; set; }

        public int TotalWin { get; set; }

        public int ValidBetAmount { get; set; }
    }
}