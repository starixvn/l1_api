namespace L1.Features.Partners.PlaceBet
{
    public class PartnerPlaceBetResult : PartnerResult
    {
        public long? ObTransId { get; set; }

        public int? ObCustId { get; set; }

        public decimal Balance { get; set; }

        public decimal MinBet { get; set; }

        public decimal MaxBet { get; set; }
    }
}