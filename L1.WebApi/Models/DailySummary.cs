using System;

namespace L1.WebApi.Models
{
    public class DailySummary
    {
        public Auth Auth { get; set; }

        public string Seq { get; set; }

        public DateTimeOffset SummarizedDate { get; set; }
    }
}