namespace L1.Features.PartnerCommunicators.HitJackpot
{
    public class HitJackpotCall : PartnerCall
    {
        public HitJackpotCall(
            object playerId,
            string siteId,
            int gameRoundId,
            int gameTypeId,
            decimal winningAmount,
            string siteName) : base(playerId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            WinningAmount = winningAmount;
            SiteName = siteName;
        }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public decimal WinningAmount { get; set; }

        public string SiteName { get; set; }
    }
}