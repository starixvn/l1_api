namespace L1.Features.GameServerCommunicators.RetrieveEndGameInfo
{
    public class RetrieveEndGameInfoRequest : GameServerRequest
    {
        public RetrieveEndGameInfoRequest(int gameRoundId, int gameTypeId, int obCustId, string siteId) : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
        }

        public int GameRoundId { get; }

        public int GameTypeId { get; }
    }
}