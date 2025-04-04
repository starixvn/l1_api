using NATS.Client.JetStream;

namespace L1.Shared.Configurations
{
    public class StreamSettings
    {
        public string Host { get; set; }

        public string StreamName { get; set; }

        public string Prefix { get; set; }

        public string SubjectPrefix { get; set; }

        public bool Enable { get; set; }

        public int PublishTimeoutMs { get; set; } = 500;

        // Stream configuration settings
        public string StorageType { get; set; } = "Memory";
        public string RetentionPolicy { get; set; } = "WorkQueue";
        public string DiscardPolicy { get; set; } = "Old";
        public int MaxAgeHours { get; set; } = 1;
        public int MaxSizeMB { get; set; } = 100;
        public int DuplicateWindowMinutes { get; set; } = 2;
        public int MaxMessages { get; set; } = 100_000;
        public int Replicas { get; set; } = 1;
        
        public RetentionPolicy GetRetentionPolicy() =>
            RetentionPolicy?.ToLower() switch
            {
                "interest" => NATS.Client.JetStream.RetentionPolicy.Interest,
                "limits" => NATS.Client.JetStream.RetentionPolicy.Limits,
                _ => NATS.Client.JetStream.RetentionPolicy.WorkQueue
            };

        public DiscardPolicy GetDiscardPolicy() =>
            DiscardPolicy?.ToLower() switch
            {
                "new" => NATS.Client.JetStream.DiscardPolicy.New,
                _ => NATS.Client.JetStream.DiscardPolicy.Old
            };
    }
}