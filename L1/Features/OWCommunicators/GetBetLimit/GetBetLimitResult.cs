namespace L1.Features.OWCommunicators.GetBetLimit
{
    public class GetBetLimitResult : OWResult
    {
        public decimal MinBet { get; set; }

        public decimal MaxBet { get; set; }
    }
}