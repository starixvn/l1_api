namespace L1.Shared.Configurations
{
    public class StreamSettings
    {
        public string Host { get; set; }

        public string StreamName { get; set; }

        public string Postfix { get; set; }

        public string SubjectPrefix { get; set; }

        public bool Enable { get; set; }

        public int PublishTimeoutMs { get; set; } = 500;

        // Stream Configuration for ack/nak pattern support
        public string StorageType { get; set; } = "File"; // File or Memory

        public string RetentionPolicy { get; set; } = "Limits"; // Limits, Interest, WorkQueue

        public string DiscardPolicy { get; set; } = "Old"; // Old, New

        // Enhanced defaults for new streams with ack/nak support
        public int MaxAgeHours { get; set; } = 24; // Better persistence for ack/nak

        public long MaxMessages { get; set; } = 1_000_000; // Higher capacity for ack/nak

        public long MaxBytes { get; set; } = 1_073_741_824; // 1GB default

        public int Replicas { get; set; } = 1;

        public long DuplicateWindowMs { get; set; } = 120_000; // 2 minutes for duplicate detection

        public bool NoAck { get; set; } = false; // Ensure ack/nak support

        public bool AllowRollup { get; set; } = false;

        public bool DenyDelete { get; set; } = false;

        public bool DenyPurge { get; set; } = false;

        // Backward compatibility: Use safe defaults for missing properties
        public string GetEffectiveStorageType() => string.IsNullOrEmpty(StorageType) ? "File" : StorageType;
        
        public string GetEffectiveRetentionPolicy() => string.IsNullOrEmpty(RetentionPolicy) ? "Limits" : RetentionPolicy;
        
        public string GetEffectiveDiscardPolicy() => string.IsNullOrEmpty(DiscardPolicy) ? "Old" : DiscardPolicy;

    }
}