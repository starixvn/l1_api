using L1.Features.OWCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        OWServiceSettings OWServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public OWServiceSettings OWServiceSettings { get; set; }
    }
}