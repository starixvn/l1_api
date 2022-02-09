using System;

namespace L1.Features.SboCommunicators
{
    public class SboServiceSettings
    {
        public Uri BaseUrl { get; set; }

        public string SecretKey { get; set; }

        public string HMACKey { get; set; }

        public SboEndPoints Endpoints { get; set; }
    }

    public class SboEndPoints
    {
        public string EnterPortal { get; set; }

        public string GetBalance { get; set; }

        public string PlaceBet { get; set; }

        public string EndGame { get; set; }

        public string VoidGame { get; set; }

        public string CheckMaintenance { get; set; }

        public string GetBetLimit { get; set; }
    }
}