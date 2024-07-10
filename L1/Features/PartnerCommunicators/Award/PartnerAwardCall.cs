namespace L1.Features.PartnerCommunicators.Award
{
    public class PartnerAwardCall : PartnerCall
    {
        public PartnerAwardCall(
            string playerId,
            int gameTypeId,
            decimal amount,
            int promotionId) : base(playerId)
        {
            GameTypeId = gameTypeId;
            Amount = amount;
            PromotionId = promotionId;
        }

        public decimal Amount { get; set; }

        public int GameTypeId { get; set; }

        public int PromotionId { get; set; }
    }
}