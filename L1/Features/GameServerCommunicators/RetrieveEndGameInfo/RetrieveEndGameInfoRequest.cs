namespace L1.Features.GameServerCommunicators.RetrieveEndGameInfo
{
    public class RetrieveEndGameInfoRequest : GameServerRequest
    {
        public RetrieveEndGameInfoRequest(int gameRoundId, byte gameTypeId, int obCustId, string siteId, bool freeBet)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            ObCustId = obCustId;
            SiteId = siteId;
            FreeBet = freeBet;
        }

        public int GameRoundId { get; }

        public byte GameTypeId { get; }

        public int ObCustId { get; }

        public string SiteId { get; }

        public bool FreeBet { get; }
    }
}