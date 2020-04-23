using L1.Features.GameServerCommunicators;

namespace L1.Shared.Configurations
{
    public partial interface IAppSettings
    {
        GameServerSettings GameServerSettings { get; set; }
    }

    public partial class AppSettings : IAppSettings
    {
        public GameServerSettings GameServerSettings { get; set; }
    }
}