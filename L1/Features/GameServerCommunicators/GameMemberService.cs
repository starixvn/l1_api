using System.Threading.Tasks;
using L1.Features.GameServerCommunicators.RetrieveEndGameInfo;
using L1.Features.GameServerCommunicators.UnderMaintenance;

namespace L1.Features.GameServerCommunicators
{
    public interface IGameMemberService
    {
        Task<GameServerResponse<RetrieveEndGameInfoResult>> RetrieveEndGameInfoAsync(RetrieveEndGameInfoRequest request);

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

        public Task<GameServerResponse<RetrieveEndGameInfoResult>> RetrieveEndGameInfoAsync(RetrieveEndGameInfoRequest request)
            => gameServerService.Post<RetrieveEndGameInfoResult>(gameServerEndPoints.RetrieveEndGameInfo, request);

        public Task UnderMaintenance(MaintenanceRequest request)
            => gameServerService.PostAndForget(gameServerEndPoints.UnderMaintenance, request);
    }
}