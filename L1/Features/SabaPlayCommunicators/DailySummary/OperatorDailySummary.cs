namespace L1.Features.SabaPlayCommunicators.DailySummary
{
    public class OperatorDailySummary
    {
        public string OperatorId { get; set; }
        public string TargetDate { get; set; }
        public string SiteId { get; set; }
        public string CurrencyIso { get; set; }
        public decimal BetCount { get; set; }
        public decimal ActualStake { get; set; }
        public decimal WinLose { get; set; }
    }
}