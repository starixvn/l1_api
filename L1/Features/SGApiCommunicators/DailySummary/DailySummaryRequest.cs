using System;

namespace L1.Features.SGApiCommunicators
{
    public class DailySummaryRequest : SGRequest
    {
        public DateTimeOffset SummarizedDate { get; set; }
    }
}