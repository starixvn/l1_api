using L1.Features.GameServerCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        GameServerSetting GameServerSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public GameServerSetting GameServerSettings { get; set; }
    }
}