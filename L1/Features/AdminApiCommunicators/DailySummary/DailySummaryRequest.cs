using System;

namespace L1.Features.AdminApiCommunicators.DailySummary
{
    public class DailySummaryRequest : AdminApiRequest
    {
        public DailySummaryRequest(string partnerId, DateTimeOffset summarizedDate)
        {
            PartnerId = partnerId;
            SummarizedDate = summarizedDate;
        }

        public string PartnerId { get; }

        public DateTimeOffset SummarizedDate { get; }
    }
}