namespace L1.Features.PartnerCommunicators.VoidGame
{
    public class PartnerVoidGameCall : PartnerCall
    {
        public PartnerVoidGameCall(string playerId, string siteId, int gameRoundId, int gameTypeId, string reason, bool freeBet) : base(playerId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            Reason = reason;
            FreeBet = freeBet;
        }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public string Reason { get; set; }

        public bool FreeBet { get; set; }
    }
}