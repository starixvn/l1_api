using System;

namespace OW.ApiSimulator.Models.OW
{
    public class CheckMaintenanceResult : OWResult
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}