using System;

namespace L1.Shared.Configurations
{
    public class OWServiceSettings
    {
        public Uri BaseUrl { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string NotifyLoginEndPoint { get; set; }

        public string GetBalanceEndPoint { get; set; }

        public string PlaceBetEndPoint { get; set; }

        public string EndGameEndPoint { get; set; }

        public string VoidGameEndPoint { get; set; }
    }
}