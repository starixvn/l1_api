namespace L1.Features.PartnerCommunicators.VoidGame
{
    public class PartnerVoidGameCall : PartnerCall
    {
        public PartnerVoidGameCall(int obCustId, string siteId, int gameRoundId, byte gameTypeId, string reason) : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            Reason = reason;
        }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public string Reason { get; set; }
    }
}