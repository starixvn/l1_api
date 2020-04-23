using System;

namespace L1.Features.AdminApiCommunicators.UnderMaintenance
{
    public class MaintenanceRequest : AdminApiRequest
    {
        public MaintenanceRequest(DateTimeOffset? startTime, DateTimeOffset? endTime, bool isUM)
        {
            StartTime = startTime;
            EndTime = endTime;
            IsUM = isUM;
        }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsUM { get; }
    }
}