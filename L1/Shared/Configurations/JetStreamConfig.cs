namespace L1.Shared.Configurations
{
    public class JetStreamConfig
    {
        public long MaxBytes { get; set; } // Maximum number of bytes
        public long MaxAge { get; set; } // Maximum age of messages in milliseconds
        public string[] Subjects { get; set; } // Subjects to subscribe to
        public string Name { get; set; } // Stream name
        public string Retention { get; set; } // Retention policy (e.g., "limits", "interest", "workqueue")
    }
}