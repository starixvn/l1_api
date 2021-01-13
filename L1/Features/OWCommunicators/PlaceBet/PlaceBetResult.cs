namespace L1.Features.OWCommunicators.PlaceBet
{
    public class PlaceBetResult : OWResult
    {
        public long? ObTransId { get; set; }

        public int? ObCustId { get; set; }

        public decimal Balance { get; set; }

        public decimal MinBet { get; set; }

        public decimal MaxBet { get; set; }
    }
}