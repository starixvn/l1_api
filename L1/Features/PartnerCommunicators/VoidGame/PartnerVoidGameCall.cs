namespace L1.Features.PartnerCommunicators.VoidGame
{
    public class PartnerVoidGameCall : PartnerCall
    {
        public PartnerVoidGameCall(string playerId, string siteId, int gameRoundId, byte gameTypeId, string reason, bool freeBet) : base(playerId, siteId)
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