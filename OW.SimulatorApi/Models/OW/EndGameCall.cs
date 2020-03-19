namespace OW.SimulatorApi.Models.OW
{
    public class EndGameCall : OWCall
    {
        public int GameRoundId { get; set; }

        public int TotalAmount { get; set; }

        public int TotalWin { get; set; }

        public int ValidBetAmount { get; set; }
    }
}
