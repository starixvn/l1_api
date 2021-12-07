using L1.Features.SboCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        SboServiceSettings SboServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public SboServiceSettings SboServiceSettings { get; set; }
    }
}