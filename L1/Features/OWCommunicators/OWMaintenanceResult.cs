using System;

namespace L1.Features.OWCommunicators
{
    public class OWMaintenanceResult : OWResult
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}