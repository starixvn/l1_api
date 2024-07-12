namespace L1.Features.PartnerCommunicators.Award
{
    public class PartnerAwardCall : PartnerCall
    {
        public PartnerAwardCall(
            string playerId,
            int gameTypeId,
            decimal amount,
            int promotionId,
            string currency) : base(playerId)
        {
            GameTypeId = gameTypeId;
            Amount = amount;
            PromotionId = promotionId;
            Currency = currency;
        }

        public decimal Amount { get; set; }

        public int GameTypeId { get; set; }

        public int PromotionId { get; set; }

        public string Currency { get; set; }
    }
}