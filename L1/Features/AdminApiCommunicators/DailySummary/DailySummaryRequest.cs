using System;

namespace L1.Features.AdminApiCommunicators.DailySummary
{
    public class DailySummaryRequest : AdminApiRequest
    {
        public DailySummaryRequest(DateTimeOffset summarizedDate)
        {
            SummarizedDate = summarizedDate;
        }

        public DateTimeOffset SummarizedDate { get; }
    }
}