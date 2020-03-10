using System.Collections.Generic;

namespace L1.Shared.Configurations
{
    public interface IAppSettings
    {
        IEnumerable<LicenseeConfig> LicenseeConfigs { get; set; }

        MongoDBSettings MongoDBSettings { get; set; }

        string IdentityServer { get; set; }

        IEnumerable<string> IdentityAudiences { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public IEnumerable<LicenseeConfig> LicenseeConfigs { get; set; }

        public MongoDBSettings MongoDBSettings { get; set; }

        public string IdentityServer { get; set; }

        public IEnumerable<string> IdentityAudiences { get; set; }
    }
}