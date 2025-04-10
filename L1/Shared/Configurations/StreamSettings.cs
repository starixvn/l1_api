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

        public int MaxAgeHours { get; set; } = 1;

        public int MaxMessages { get; set; } = 100_000;

        public int Replicas { get; set; } = 1;

    }
}