using System;

namespace OW.ApiSimulator.Models.OW
{
    public class OWResponse
    {
        public DateTimeOffset TimeStamp { get; set; }

        public OWResult Result { get; set; }
    }
}
