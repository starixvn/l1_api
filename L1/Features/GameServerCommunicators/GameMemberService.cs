using System.Threading.Tasks;
using L1.Features.GameServerCommunicators.DailySummary;
using L1.Features.GameServerCommunicators.RetrieveEndGameInfo;
using L1.Features.GameServerCommunicators.UnderMaintenance;

namespace L1.Features.GameServerCommunicators
{
    public interface IGameMemberService
    {
        Task<GameServerResponse<RetrieveEndGameInfoResult>> RetrieveEndGameInfoAsync(RetrieveEndGameInfoRequest request);

        Task<GameServerResponse<DailySummaryResult>> DailySummary(DailySummaryRequest request);

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

        public async Task<GameServerResponse<RetrieveEndGameInfoResult>> RetrieveEndGameInfoAsync(RetrieveEndGameInfoRequest request)
        {
            var response = await gameServerService.Post<RetrieveEndGameInfoResult>(gameServerEndPoints.RetrieveEndGameInfo, request);

            return response;
        }

        public async Task<GameServerResponse<DailySummaryResult>> DailySummary(DailySummaryRequest request)
        {
            var response = await gameServerService.Post<DailySummaryResult>(gameServerEndPoints.DailySummary, request);

            return response;
        }

        public Task UnderMaintenance(MaintenanceRequest request)
            => gameServerService.PostAndForget(gameServerEndPoints.UnderMaintenance, request);
    }
}