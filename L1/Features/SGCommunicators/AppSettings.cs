using L1.Features.SGCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        SGServiceSettings SGServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public SGServiceSettings SGServiceSettings { get; set; }
    }
}