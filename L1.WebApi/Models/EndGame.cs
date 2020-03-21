namespace L1.WebApi.Models
{
    public class EndGame
    {
        public int MemberId { get; set; }

        public string Seq { get; set; }

        public string SiteId { get; set; }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalWin { get; set; }

        public int ValidBetAmount { get; set; }
    }
}