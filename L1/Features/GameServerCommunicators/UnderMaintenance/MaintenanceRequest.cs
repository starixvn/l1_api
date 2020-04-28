using System;

namespace L1.Features.GameServerCommunicators.UnderMaintenance
{
    public class MaintenanceRequest : GameServerRequest
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