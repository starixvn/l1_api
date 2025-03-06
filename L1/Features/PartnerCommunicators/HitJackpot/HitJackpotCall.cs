namespace L1.Features.PartnerCommunicators.HitJackpot
{
    public class HitJackpotCall : PartnerCall
    {
        public HitJackpotCall(
            string playerId,
            string siteId,
            int gameRoundId,
            int gameTypeId,
            decimal winningAmount) : base(playerId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            WinningAmount = winningAmount;
        }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public decimal WinningAmount { get; set; }
    }
}