namespace L1.Features.GameServerCommunicators
{
    public class GameServerRequest
    {
        public GameServerRequest(int obCustId, int siteId, string seq)
        {
            ObCustId = obCustId;
            SiteId = siteId;
            Seq = seq;
        }

        public int ObCustId { get; }

        public int SiteId { get; }

        public string Seq { get; }
    }
}