using NATS.Client.JetStream;

namespace L1.Shared.Configurations
{
    public class StreamSettings
    {
        // Constants for policy values
        public const string StorageTypeMemory = "Memory";
        public const string RetentionPolicyWorkQueue = "WorkQueue";
        public const string RetentionPolicyInterest = "Interest";
        public const string RetentionPolicyLimits = "Limits";
        public const string DiscardPolicyOld = "Old";
        public const string DiscardPolicyNew = "New";

        public string Host { get; set; }

        public string StreamName { get; set; }

        public string Prefix { get; set; }

        public string SubjectPrefix { get; set; }

        public bool Enable { get; set; }

        public int PublishTimeoutMs { get; set; } = 500;

        // Stream configuration settings
        public string StorageType { get; set; } = StorageTypeMemory;
        public string RetentionPolicy { get; set; } = RetentionPolicyWorkQueue;
        public string DiscardPolicy { get; set; } = DiscardPolicyOld;
        public int MaxAgeHours { get; set; } = 1;
        public int MaxSizeMB { get; set; } = 100;
        public int DuplicateWindowMinutes { get; set; } = 2;
        public int MaxMessages { get; set; } = 100_000;
        public int Replicas { get; set; } = 1;
        
        public RetentionPolicy GetRetentionPolicy() =>
            RetentionPolicy?.ToLower() switch
            {
                var policy when policy == RetentionPolicyInterest.ToLower() => NATS.Client.JetStream.RetentionPolicy.Interest,
                var policy when policy == RetentionPolicyLimits.ToLower() => NATS.Client.JetStream.RetentionPolicy.Limits,
                _ => NATS.Client.JetStream.RetentionPolicy.WorkQueue
            };

        public DiscardPolicy GetDiscardPolicy()
        {
            return DiscardPolicy?.ToLower() == DiscardPolicyNew.ToLower()
                ? NATS.Client.JetStream.DiscardPolicy.New
                : NATS.Client.JetStream.DiscardPolicy.Old;
        }
    }
}