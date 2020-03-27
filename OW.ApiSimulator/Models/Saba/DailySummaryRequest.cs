using System;

namespace OW.ApiSimulator.Models.Saba
{
    public class DailySummaryRequest : SabaRequest
    {
        public DateTimeOffset SummarizedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}