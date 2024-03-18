using System;

namespace L1.Features.SabaPlayCommunicators
{
    public class SabaPlayServiceSettings
    {
        public Uri BaseUrl { get; set; }

        public string SecretKey { get; set; }

        public SabaPlayEndPoints Endpoints { get; set; }

        public string Proxy { get; set; }
    }

    public class SabaPlayEndPoints
    {
        public string EnterPortal { get; set; }

        public string GetBalance { get; set; }

        public string PlaceBet { get; set; }

        public string EndGame { get; set; }

        public string VoidGame { get; set; }

        public string CheckMaintenance { get; set; }

        public string HitJackpot { get; set; }

        public string GetBetLimit { get; set; }
    }
}