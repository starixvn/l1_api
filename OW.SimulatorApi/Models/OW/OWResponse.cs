using System;

namespace OW.SimulatorApi.Models.OW
{
    public class OWResponse
    {
        public DateTimeOffset TimeStamp { get; set; }

        public OWResult Result { get; set; }
    }
}
