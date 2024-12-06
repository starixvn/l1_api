using System;
using L1.Features.OWCommunicators;

namespace L1.Features.SKCommunicators
{
    public class SKServiceSettings
    {
        public Uri BaseUrl { get; set; }      

        public string SecretKey { get; set; }

        public string Proxy { get; set; }

        public SKEndPoints Endpoints { get; set; }
    }

    public class SKEndPoints : OWEndPoints
    {}
}