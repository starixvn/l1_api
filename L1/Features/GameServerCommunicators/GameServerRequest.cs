namespace L1.Features.GameServerCommunicators
{
    public class GameServerRequest
    {
        public GameServerRequest(int obCustId, string siteId)
        {
            ObCustId = obCustId;
            SiteId = siteId;
        }

        public int ObCustId { get; }

        public string SiteId { get; }
    }
}