using L1.Features.AdminApiCommunicators;
using L1.Features.SGApiCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        SGApiSettings SGApiSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public SGApiSettings SGApiSettings { get; set; }
    }
}