namespace L1.Features.PartnerCommunicators.HitJackpot
{
    public class HitJackpotCall : PartnerCall
    {
        public HitJackpotCall(
            int obCustId,
            string siteId,
            int gameRoundId,
            byte gameTypeId,
            decimal winningAmount) : base(obCustId, siteId)
        {
            GameRoundId = gameRoundId;
            GameTypeId = gameTypeId;
            WinningAmount = winningAmount;
        }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public decimal WinningAmount { get; set; }
    }
}