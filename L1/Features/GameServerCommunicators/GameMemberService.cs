using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.DailySummary;
using L1.Features.GameServerCommunicators.RetrieveEndGameInfo;
using L1.Features.GameServerCommunicators.UnderMaintenance;

namespace L1.Features.GameServerCommunicators
{
    public interface IGameMemberService
    {
        Task<GameServerResponse<RetrieveEndGameInfoResult>> RetrieveEndGameInfoAsync(RetrieveEndGameInfoRequest request);

        Task<AdminApiResponse<DailySummaryResult>> DailySummary(DailySummaryRequest request);

        Task UnderMaintenance(MaintenanceRequest request);
    }

    public class GameMemberService : IGameMemberService
    {
        private readonly IGameServerService gameServerService;
        private readonly IAdminApiService adminApiService;
        private readonly GameServerEndPoints gameServerEndPoints;
        private readonly AdminApiEndPoints adminApiEndPoints;

        public GameMemberService(
            IGameServerService gameServerService,
            IAdminApiService adminApiService,
            GameServerEndPoints gameServerEndPoints,
            AdminApiEndPoints adminApiEndPoints)
        {
            this.gameServerService = gameServerService;
            this.adminApiService = adminApiService;
            this.gameServerEndPoints = gameServerEndPoints;
            this.adminApiEndPoints = adminApiEndPoints;
        }

        public Task<GameServerResponse<RetrieveEndGameInfoResult>> RetrieveEndGameInfoAsync(RetrieveEndGameInfoRequest request)
            => gameServerService.Post<RetrieveEndGameInfoResult>(gameServerEndPoints.RetrieveEndGameInfo, request);

        public Task<AdminApiResponse<DailySummaryResult>> DailySummary(DailySummaryRequest request)
            => adminApiService.Post<DailySummaryResult>(adminApiEndPoints.DailySummary, request);

        public Task UnderMaintenance(MaintenanceRequest request)
            => gameServerService.PostAndForget(gameServerEndPoints.UnderMaintenance, request);
    }
}