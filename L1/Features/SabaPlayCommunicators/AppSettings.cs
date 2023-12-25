using L1.Features.SabaPlayCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        SabaPlayServiceSettings SabaPlayServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public SabaPlayServiceSettings SabaPlayServiceSettings { get; set; }
    }
}