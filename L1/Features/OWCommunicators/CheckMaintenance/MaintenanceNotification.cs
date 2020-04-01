namespace L1.Features.OWCommunicators.CheckMaintenance
{
    internal class MaintenanceNotification
    {
        public MaintenanceNotification(bool isUM)
        {
            IsUM = isUM;
        }

        public bool IsUM { get; }
    }
}