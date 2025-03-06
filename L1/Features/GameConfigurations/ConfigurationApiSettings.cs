using System;

namespace L1.Features.GameConfigurations
{
    public class ConfigurationApiSettings
    {
        public Uri BaseUrl { get; set; }

        public ConfigurationEndPoints Endpoints { get; set; }
    }

    public class ConfigurationEndPoints
    {
        public string DetailGameConfiguration { get; set; }
    }
}
