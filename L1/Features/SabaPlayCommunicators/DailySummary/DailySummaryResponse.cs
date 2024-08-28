using System;
using System.Collections.Generic;
using L1.Features.PartnerCommunicators;
using Newtonsoft.Json;

namespace L1.Features.SabaPlayCommunicators.DailySummary
{
    public class DailySummaryResponse : PartnerResponse<DailySummaryResult>
    {
    }

    public class DailySummaryResult : PartnerResult
    {
        [JsonProperty("Timestamp")]
        public DateTimeOffset TimeStamp { get; set; }

        public DailySummaryPayload Payload { get; set; }
    }

    public class DailySummaryPayload
    {
        public string TargetDate { get; set; }
        public IEnumerable<DailySummarySiteData> SiteData { get; set; }
    }

    public class DailySummarySiteData
    {
        public string SiteId { get; set; }
        public IEnumerable<DailySummaryBettingData> BettingData { get; set; }
    }

    public class DailySummaryBettingData
    {
        public string CurrencyIso { get; set; }
        public decimal BetCount { get; set; }
        public decimal ActualStake { get; set; }
        public decimal WinLose { get; set; }
    }
}