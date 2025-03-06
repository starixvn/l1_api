namespace L1.WebApi.Models
{
    public class RetrieveEndRoundInfo
    {
        public Auth Auth { get; set; }

        public string Seq { get; set; }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public string PlayerId { get; set; }

        public string SiteId { get; set; }
    }
}