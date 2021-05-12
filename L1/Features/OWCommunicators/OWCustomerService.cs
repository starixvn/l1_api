using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.GetBetLimit;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
using L1.Shared.Configurations;
using L1.Shared.Constants;

namespace L1.Features.OWCommunicators
{
    public interface IOWCustomerService
    {
        Task<OWResponse<EnterPortalResult>> EnterPortal(EnterPortalCall call);

        Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call);

        Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call, byte system = Systems.Cocos);

        Task<OWResponse<EndGameResult>> EndGame(EndGameCall call);

        Task<OWResponse<VoidGameResult>> VoidGame(VoidGameCall call);

        Task<OWResponse<GetBetLimitResult>> GetBetLimit(GetBetLimitCall call);
    }

    public class OWCustomerService : IOWCustomerService
    {
        private readonly IOWService oWService;
        private readonly ILogService logService;
        private readonly IAppSettings appSettings;
        private readonly IAdminApiService adminApiSerivce;
        private readonly OWEndPoints endpoints;

        public OWCustomerService(
            IOWService oWService,
            ILogService logService,
            IAppSettings appSettings,
            IAdminApiService adminApiSerivce,
            OWEndPoints endpoints)
        {
            this.oWService = oWService;
            this.logService = logService;
            this.appSettings = appSettings;
            this.adminApiSerivce = adminApiSerivce;
            this.endpoints = endpoints;
        }

        public Task<OWResponse<EnterPortalResult>> EnterPortal(EnterPortalCall call)
            => oWService.Post<EnterPortalResult>(endpoints.EnterPortal, call);

        public Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call)
            => oWService.Post<GetBalanceResult>(endpoints.GetBalance, call);

        public async Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call, byte system = Systems.Cocos)
        {
            var response = await oWService.Post<PlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(response, system);

                if (call.Amount >= appSettings.HighStake)
                {
                    _ = Task.Run(() => adminApiSerivce.NotifyHighStake(new NotifyHighStakeRequest(call.ObCustId, call.GameTypeId, call.Amount, call.Currency)));
                }
            }

            return response;
        }

        public async Task<OWResponse<EndGameResult>> EndGame(EndGameCall call)
        {
            var response = await oWService.Post<EndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(response);
            }

            return response;
        }

        public async Task<OWResponse<VoidGameResult>> VoidGame(VoidGameCall call)
        {
            var response = await oWService.Post<VoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(response);
            }

            return response;
        }

        public async Task<OWResponse<GetBetLimitResult>> GetBetLimit(GetBetLimitCall call)
        {
            var response = await oWService.Post<GetBetLimitResult>(endpoints.GetBetLimit, call);

            return response;
        }
    }
}