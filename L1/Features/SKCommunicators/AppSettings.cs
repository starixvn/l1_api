using L1.Features.SKCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        SKServiceSettings SKServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public SKServiceSettings SKServiceSettings { get; set; }
    }
}