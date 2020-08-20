using System.Threading.Tasks;
using L1.Features.GameServerCommunicators.UnderMaintenance;

namespace L1.Features.GameServerCommunicators
{
    public interface IGameMemberService
    {
        Task UnderMaintenance(MaintenanceRequest request);
    }

    public class GameMemberService : IGameMemberService
    {
        private readonly IGameServerService gameServerService;
        private readonly GameServerEndPoints gameServerEndPoints;

        public GameMemberService(IGameServerService gameServerService, GameServerEndPoints gameServerEndPoints)
        {
            this.gameServerService = gameServerService;
            this.gameServerEndPoints = gameServerEndPoints;
        }

        public Task UnderMaintenance(MaintenanceRequest request)
            => gameServerService.PostAndForget(gameServerEndPoints.UnderMaintenance, request);
    }
}