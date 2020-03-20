namespace L1.Features.OWCommunicators.VoidGame
{
    public class VoidGameCall : OWCall
    {
        public VoidGameCall(string seq, int obCustId, string siteId, int gameRoundId, int obTransId, string reason) : base(seq, obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            ObTransId = obTransId;
            Reason = reason;
        }

        public int GameRoundId { get; }

        public int ObTransId { get; }

        public string Reason { get; }
    }
}