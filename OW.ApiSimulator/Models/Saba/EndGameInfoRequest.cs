namespace OW.ApiSimulator.Models.Saba
{
    public class EndGameInfoRequest : SabaRequest
    {
        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public int ObCustId { get; set; }

        public string SiteId { get; set; }
    }
}