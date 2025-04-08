using NATS.Client.JetStream;
using System;

namespace L1.Shared.Configurations
{
    public class StreamSettings
    {
        // Constants for policy values       
        public const string RetentionPolicyWorkQueue = "WorkQueue";
        public const string RetentionPolicyInterest = "Interest";
        public const string RetentionPolicyLimits = "Limits";
        public const string DiscardPolicyOld = "Old";
        public const string DiscardPolicyNew = "New";

        public string Host { get; set; }

        public string StreamName { get; set; }

        public string Postfix { get; set; }

        public string SubjectPrefix { get; set; }

        public bool Enable { get; set; }

        public int PublishTimeoutMs { get; set; } = 500;

        // Stream configuration settings
        public string RetentionPolicy { get; set; } = RetentionPolicyWorkQueue;
        public string DiscardPolicy { get; set; } = DiscardPolicyOld;
        public int MaxAgeHours { get; set; } = 1;
        public int MaxSizeMB { get; set; } = 100;
        public int DuplicateWindowMinutes { get; set; } = 2;
        public int MaxMessages { get; set; } = 100_000;
        public int Replicas { get; set; } = 1;
        
        public RetentionPolicy GetRetentionPolicy() =>
            RetentionPolicy switch
            {
                var policy when String.Equals(policy, RetentionPolicyInterest, StringComparison.OrdinalIgnoreCase) => NATS.Client.JetStream.RetentionPolicy.Interest,
                var policy when String.Equals(policy, RetentionPolicyLimits, StringComparison.OrdinalIgnoreCase) => NATS.Client.JetStream.RetentionPolicy.Limits,
                _ => NATS.Client.JetStream.RetentionPolicy.WorkQueue
            };

        public DiscardPolicy GetDiscardPolicy()
        {
            return String.Equals(DiscardPolicy, DiscardPolicyNew, StringComparison.OrdinalIgnoreCase)
                ? NATS.Client.JetStream.DiscardPolicy.New
                : NATS.Client.JetStream.DiscardPolicy.Old;
        }
    }
}