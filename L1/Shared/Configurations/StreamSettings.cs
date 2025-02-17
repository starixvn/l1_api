namespace L1.Shared.Configurations
{
    public class StreamSettings
    {
        public string Host { get; set; } // nats://10.10.50.122:4222
        public JetStreamConfig JetStreamConfig { get; set; } // JetStream configuration
    }
}