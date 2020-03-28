namespace L1.Features.OWCommunicators.PlaceBet
{
    public class PlaceBetResult : OWResult
    {
        public long? ObTransId { get; set; }

        public int? ObCustId { get; set; }

        public decimal Balance { get; set; }
    }
}