using System;
using L1.Features.SboCommunicators;

namespace L1.Features.HotdogCommunicators
{
    public class HotdogServiceSettings
    {
        public Uri BaseUrl { get; set; }

        public string SecretKey { get; set; }

        public string Proxy { get; set; }

        public HotdogEndPoints Endpoints { get; set; }
    }

    public class HotdogEndPoints : SboEndPoints
    {
    }
}