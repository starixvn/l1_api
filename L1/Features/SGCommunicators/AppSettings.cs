using L1.Features.SGCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        OWSGServiceSettings OWSGServiceSettings { get; set; }
    }
    
    public partial class AppSettings : IAppSettings
    {
        public OWSGServiceSettings OWSGServiceSettings { get; set; }
    }
}