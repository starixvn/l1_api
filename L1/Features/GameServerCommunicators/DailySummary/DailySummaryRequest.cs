using System;

namespace L1.Features.GameServerCommunicators.DailySummary
{
    public class DailySummaryRequest : GameServerRequest
    {
        public DailySummaryRequest(DateTimeOffset summarizedDate)
        {
            SummarizedDate = summarizedDate;
        }

        public DateTimeOffset SummarizedDate { get; }
    }
}