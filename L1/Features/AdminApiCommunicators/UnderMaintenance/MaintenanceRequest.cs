using System;

namespace L1.Features.AdminApiCommunicators.UnderMaintenance
{
    public class MaintenanceRequest : AdminApiRequest
    {
        public MaintenanceRequest(DateTimeOffset? startTime, DateTimeOffset? endTime, bool isUM, string partnerId)
        {
            StartTime = startTime;
            EndTime = endTime;
            IsUM = isUM;
            PartnerId = partnerId;
        }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsUM { get; }

        public string PartnerId { get; }
    }
}