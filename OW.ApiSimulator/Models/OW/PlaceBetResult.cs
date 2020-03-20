namespace OW.ApiSimulator.Models.OW
{
    public class PlaceBetResult : OWResult
    {
        public int? ObTransId { get; set; }

        public int? ObCustId { get; set; }

        public double Balance { get; set; }
    }
}