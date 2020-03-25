namespace L1.Features.GameServerCommunicators
{
    public class GameServerRequest
    {
        public GameServerRequest(int obCustId, int siteId)
        {
            ObCustId = obCustId;
            SiteId = siteId;
        }

        public int ObCustId { get; }

        public int SiteId { get; }
    }
}