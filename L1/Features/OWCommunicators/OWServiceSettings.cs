using System;

namespace L1.Features.OWCommunicators
{
    public class OWServiceSettings
    {
        public Uri BaseUrl { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public OWEndPoints Endpoints { get; set; }
    }

    public class OWEndPoints
    {
        public string EnterPortal { get; set; }

        public string GetBalance { get; set; }

        public string PlaceBet { get; set; }

        public string EndGame { get; set; }

        public string VoidGame { get; set; }

        public string CheckMaintenance { get; set; }
    }
}