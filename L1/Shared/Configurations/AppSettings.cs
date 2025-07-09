using System.Collections.Generic;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        int TokenExpiredSeconds { get; set; }

        MongoDBSettings MongoDBSettings { get; set; }

        public int HighStake { get; set; }

        PartnerIds PartnerIds { get; set; }

        LogEnabling LogEnabling { get; set; }

        StreamSettings StreamSettings { get; set; }
        public string MicrositeClientPatternDomain { get; set; }

        public Dictionary<string, int[]> IgnoreBrowserAgentCheck { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public int TokenExpiredSeconds { get; set; }

        public MongoDBSettings MongoDBSettings { get; set; }

        public int HighStake { get; set; }

        public PartnerIds PartnerIds { get; set; }

        public LogEnabling LogEnabling { get; set; } = new();

        public StreamSettings StreamSettings { get; set; } = new();

        public string MicrositeClientPatternDomain { get; set; }

        public Dictionary<string, int[]> IgnoreBrowserAgentCheck { get; set; } = new();
    }

    public class LogEnabling
    {
        public bool EnterPortal { get; set; }
        public bool GetBetLimit { get; set; }
    }
}