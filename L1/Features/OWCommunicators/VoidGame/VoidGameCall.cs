namespace L1.Features.OWCommunicators.VoidGame
{
    public class VoidGameCall : OWCall
    {
        public VoidGameCall(string seq, int obCustId, string siteId, int gameRoundId, int gameTypeId, string reason) : base(seq, obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            Reason = reason;
        }

        public int GameRoundId { get; }

        public int GameTypeId { get; }

        public string Reason { get; }
    }
}