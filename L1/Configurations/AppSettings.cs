using System.Collections.Generic;

namespace L1.Configurations
{
    public interface IAppSettings
    {
        IEnumerable<LicenseeConfig> LicenseeConfigs { get; set; }

        string MongoDBConnectionString { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public IEnumerable<LicenseeConfig> LicenseeConfigs { get; set; }

        public string MongoDBConnectionString { get; set; }
    }

    public class LicenseeConfig
    {
        public string SiteName { get; set; }

        public string BackendApi { get; set; }
    }
}