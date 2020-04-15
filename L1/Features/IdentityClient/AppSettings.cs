using System.Collections.Generic;
using L1.Features.IdentityClient;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        IdentityServerConfigs IdentityServerConfigs { get; set; }

        IEnumerable<string> IdentityAudiences { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public IdentityServerConfigs IdentityServerConfigs { get; set; }

        public IEnumerable<string> IdentityAudiences { get; set; }
    }
}