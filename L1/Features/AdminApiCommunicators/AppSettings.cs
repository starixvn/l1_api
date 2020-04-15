using L1.Features.AdminApiCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        AdminApiSettings AdminApiSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public AdminApiSettings AdminApiSettings { get; set; }
    }
}