namespace L1.Features.OWCommunicators.PlaceBet
{
    public class PlaceBetResult : OWResult
    {
        public int? ObTransId { get; set; }

        public int? ObCustId { get; set; }

        public double Balance { get; set; }
    }
}