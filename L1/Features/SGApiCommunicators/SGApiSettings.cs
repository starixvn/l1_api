using System;

namespace L1.Features.SGApiCommunicators
{
    public class SGApiSettings
    {
        public Uri BaseUrl { get; set; }

        public SGApiEndPoints Endpoints { get; set; }
    }

    public class SGApiEndPoints
    {
        public string DailySummary { get; set; }

        public string RetrieveEndRoundInfo { get; set; }
    }
}