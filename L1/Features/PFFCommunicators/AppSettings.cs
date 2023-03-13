using L1.Features.PFFCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        PFFServiceSettings PFFServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public PFFServiceSettings PFFServiceSettings { get; set; }
    }
}