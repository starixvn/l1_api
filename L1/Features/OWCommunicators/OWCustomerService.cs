using System.Threading.Tasks;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
using L1.Shared.Configurations;

namespace L1.Features.OWCommunicators
{
    public interface IOWCustomerService
    {
        Task<OWResponse<EnterPortalResult>> EnterPortal(EnterPortalCall call);

        Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call);

        Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call);

        Task<OWResponse<EndGameResult>> EndGame(EndGameCall call);

        Task<OWResponse<VoidGameResult>> VoidGame(VoidGameCall call);
    }

    public class OWCustomerService : IOWCustomerService
    {
        private readonly IOWService oWService;
        private readonly ILogService logService;
        private readonly OWEndPoints endpoints;

        public OWCustomerService(IOWService oWService, ILogService logService, OWEndPoints endpoints)
        {
            this.oWService = oWService;
            this.logService = logService;
            this.endpoints = endpoints;
        }

        public async Task<OWResponse<EnterPortalResult>> EnterPortal(EnterPortalCall call)
        {
            return await oWService.Post<EnterPortalResult>(endpoints.EnterPortal, call);
        }

        public async Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call)
        {
            return await oWService.Post<GetBalanceResult>(endpoints.GetBalance, call);
        }

        public async Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call)
        {
            var response = await oWService.Post<PlaceBetResult>(endpoints.PlaceBet, call);

            await logService.LogBetAsync(response);

            return response;
        }

        public async Task<OWResponse<EndGameResult>> EndGame(EndGameCall call)
        {
            var response = await oWService.Post<EndGameResult>(endpoints.EndGame, call);

            await logService.LogEndGameAsync(response);

            return response;
        }

        public async Task<OWResponse<VoidGameResult>> VoidGame(VoidGameCall call)
        {
            return await oWService.Post<VoidGameResult>(endpoints.VoidGame, call);
        }
    }
}