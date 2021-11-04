using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.Partners;
using L1.Features.Partners.EndGame;
using L1.Features.Partners.EnterPortal;
using L1.Features.Partners.GetBalance;
using L1.Features.Partners.GetBetLimit;
using L1.Features.Partners.Log;
using L1.Features.Partners.PlaceBet;
using L1.Features.Partners.VoidGame;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using System.Threading.Tasks;

namespace L1.Features.OWCommunicators
{
    public class OWCustomerService : IPartnerCustomerServices
    {
        private readonly string PartnerId = PartnerIds.OW;

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

        public Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        => oWService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => oWService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);

        public Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        => oWService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub)
        {
            var response = await oWService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(PartnerId, response, system);

                if (call.Amount >= appSettings.HighStake)
                {
                    await adminApiSerivce.NotifyHighStake(new NotifyHighStakeRequest(
                        call.ObCustId,
                        call.GameTypeId,
                        call.Amount,
                        call.Currency,
                        call.ChoiceId,
                        system));
                }
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await oWService.Post<PartnerEndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(PartnerId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await oWService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(PartnerId, response);
            }

            return response;
        }
    }
}