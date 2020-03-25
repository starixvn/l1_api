using System.Threading.Tasks;
using L1.Features.GameServerCommunicators.RetrieveEndGameInfo;
using L1.Shared.Configurations;

namespace L1.Features.GameServerCommunicators
{
    public interface IGameMemberService
    {
        Task<GameServerResponse<RetrieveEndGameInfoResult>> RetrieveEndGameInfoAsync(RetrieveEndGameInfoRequest request);
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
            response.Result.Seq = request.Seq;

            return response;
        }
    }
}