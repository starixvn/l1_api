using System.Collections.Generic;

namespace L1.Shared.Configurations
{
    public interface IAppSettings
    {
        MongoDBSettings MongoDBSettings { get; set; }

        string IdentityServer { get; set; }

        int TokenExpiredSeconds { get; set; }

        IEnumerable<string> IdentityAudiences { get; set; }
    }

    public class AppSettings : IAppSettings
    {

        public MongoDBSettings MongoDBSettings { get; set; }

        public string IdentityServer { get; set; }

        public IEnumerable<string> IdentityAudiences { get; set; }

        public int TokenExpiredSeconds { get; set; }
    }
}