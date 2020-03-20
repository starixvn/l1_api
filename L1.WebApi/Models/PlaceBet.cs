namespace L1.WebApi.Models
{
    public class PlaceBet
    {
        public int MemberId { get; set; }

        public string Seq { get; set; }

        public string SiteId { get; set; }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public int ChoiceId { get; set; }

        public double Amount { get; set; }

        public string Currency { get; set; }

        public string Ip { get; set; }
    }
}