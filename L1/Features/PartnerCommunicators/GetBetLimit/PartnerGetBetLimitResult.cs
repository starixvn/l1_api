namespace L1.Features.PartnerCommunicators.GetBetLimit
{
    public class PartnerGetBetLimitResult : PartnerResult
    {
        public decimal MinBet { get; set; }

        public decimal MaxBet { get; set; }
    }
}