using System;

namespace L1.Features.SabaPlayCommunicators.DailySummary
{
    public class SabaPlayDailySummaryServiceSettings
    {
        public Uri BaseUrl { get; set; }

        public SabaPlayDailySummaryEndPoints Endpoints { get; set; }

        public string Proxy { get; set; }
    }

    public class SabaPlayDailySummaryEndPoints
    {
        public string DailySummary { get; set; }
    }
}