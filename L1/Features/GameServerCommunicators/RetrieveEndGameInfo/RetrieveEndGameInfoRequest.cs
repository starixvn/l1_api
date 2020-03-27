namespace L1.Features.GameServerCommunicators.RetrieveEndGameInfo
{
    public class RetrieveEndGameInfoRequest : GameServerRequest
    {
        public RetrieveEndGameInfoRequest(int gameRoundId, byte gameTypeId, int obCustId, string siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            ObCustId = obCustId;
            SiteId = siteId;
        }

        public int GameRoundId { get; }

        public byte GameTypeId { get; }

        public int ObCustId { get; }

        public string SiteId { get; }
    }
}