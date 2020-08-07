using System.Collections.Generic;

namespace L1.Features.AdminApiCommunicators.DailySummary
{
    public class DailySummaryResult : AdminApiResult
    {
        public string Seq { get; set; }

        public IEnumerable<SummaryItem> Summary { get; set; }
    }

    public class SummaryItem
    {
        public string Currency { get; set; }

        public decimal Stake { get; set; }

        public decimal Payout { get; set; }

        public int TicketCount { get; set; }
    }
}