using System;
using System.Collections.Generic;

namespace OW.ApiSimulator.Models.Saba
{
    public class DailySummaryResult : SabaResult
    {
        public IEnumerable<SummaryItem> Summary { get; set; }
    }

    public class SummaryItem
    {
        public DateTimeOffset EventDate { get; set; }

        public string Currency { get; set; }

        public decimal Stake { get; set; }

        public decimal Payout { get; set; }

        public int TicketCount { get; set; }
    }
}