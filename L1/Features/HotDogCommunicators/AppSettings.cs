using L1.Features.HotdogCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        HotdogServiceSettings HotdogServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public HotdogServiceSettings HotdogServiceSettings { get; set; }
    }
}