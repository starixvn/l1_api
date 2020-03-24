namespace L1.WebApi.Models
{
    public class EndGameInfoRequest
    {
        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public int ObCustId { get; set; }

        public int SiteId { get; set; }
    }
}