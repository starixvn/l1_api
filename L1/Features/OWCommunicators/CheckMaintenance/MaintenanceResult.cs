using System;

namespace L1.Features.OWCommunicators.CheckMaintenance
{
    public class MaintenanceResult : OWResult
    {
        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }

        public bool IsUnderMaintenance()
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {
                var utcNow = DateTimeOffset.UtcNow;

                return StartTime.Value <= utcNow && utcNow <= EndTime.Value;
            }

            return false;
        }

        public TimeSpan CalculateDuration()
        {
            if (StartTime.HasValue && EndTime.HasValue)
            {
                return EndTime.Value - DateTimeOffset.UtcNow;
            }

            return TimeSpan.Zero;
        }
    }
}