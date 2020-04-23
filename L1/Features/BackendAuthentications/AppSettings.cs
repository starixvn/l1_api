using System.Collections.Generic;
using L1.Features.BackendAuthentications;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        BackendAuthConfigs BackendAuthConfigs { get; set; }

        IEnumerable<string> IdentityAudiences { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public BackendAuthConfigs BackendAuthConfigs { get; set; }

        public IEnumerable<string> IdentityAudiences { get; set; }
    }
}