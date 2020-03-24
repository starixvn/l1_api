namespace L1.Features.GameServerCommunicators.RetrieveEndGameInfo
{
    public class RetrieveEndGameInfoRequest : GameServerRequest
    {
        public RetrieveEndGameInfoRequest(int gameRoundId, int gameTypeId, int obCustId, int siteId, string seq)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            ObCustId = obCustId;
            SiteId = siteId;
            Seq = seq;
        }

        public int GameRoundId { get; }

        public int GameTypeId { get; }

        public int ObCustId { get; }

        public int SiteId { get; }

        public string Seq { get; }
    }
}