namespace L1.Features.PartnerCommunicators.PlaceBet
{
    public class PartnerPlaceBetResult : PartnerResult
    {
        public long? ObTransId { get; set; }

        public dynamic PlayerId { get; set; }

        public decimal Balance { get; set; }

        public decimal MinBet { get; set; }

        public decimal MaxBet { get; set; }
    }
}