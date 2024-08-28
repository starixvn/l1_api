using System;
using L1.Features.PartnerCommunicators;
using Newtonsoft.Json;

namespace L1.Features.SabaPlayCommunicators.DailySummary
{
    public class DailySummaryCall : PartnerCall
    {
        public DailySummaryCall() : base(string.Empty, string.Empty)
        {
        }

        [JsonProperty("Timestamp")]
        public new DateTimeOffset TimeStamp { get; } = DateTimeOffset.UtcNow;

        public DailySummaryCallPayload Payload { get; set; } = new();
    }

    public class DailySummaryCallPayload
    {
        public string TargetDate { get; set; } = DateTimeOffset.UtcNow.ToString("yyyy/MM/dd");
    }
}