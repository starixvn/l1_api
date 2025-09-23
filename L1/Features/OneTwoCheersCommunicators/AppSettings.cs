using L1.Features.OneTwoCheersCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        OneTwoCheersServiceSettings OneTwoCheersServiceSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public OneTwoCheersServiceSettings OneTwoCheersServiceSettings { get; set; }
    }
}
