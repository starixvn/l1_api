using L1.Features.GameConfigurations;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        public ConfigurationApiSettings ConfigurationApiSettings { get; set; }

    }

    public partial class AppSettings : IAppSettings
    {
        public ConfigurationApiSettings ConfigurationApiSettings { get; set; }
    }
}
