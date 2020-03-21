namespace L1.WebApi.Models
{
    public class EndGame
    {
        public int MemberId { get; set; }

        public string Seq { get; set; }

        public string SiteId { get; set; }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public double TotalAmount { get; set; }

        public double TotalWin { get; set; }

        public int ValidBetAmount { get; set; }
    }
}