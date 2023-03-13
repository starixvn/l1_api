using System;
using L1.Features.SboCommunicators;

namespace L1.Features.PFFCommunicators
{
    public class PFFServiceSettings
    {
        public Uri BaseUrl { get; set; }

        public string SecretKey { get; set; }

        public PFFEndPoints Endpoints { get; set; }
    }

    public class PFFEndPoints : SboEndPoints
    {
    }
}