namespace L1.Features.OWCommunicators.VoidGame
{
    public class VoidGameCall : OWCall
    {
        public VoidGameCall(int obCustId, string siteId, int gameRoundId, byte gameTypeId, string reason, bool freeBet) : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            Reason = reason;
            FreeBet = freeBet;
        }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public string Reason { get; set; }

        public bool FreeBet { get; set; }
    }
}