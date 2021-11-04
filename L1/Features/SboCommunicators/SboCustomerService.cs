using System.Threading.Tasks;
using L1.Features.Partners;
using L1.Features.Partners.EndGame;
using L1.Features.Partners.EnterPortal;
using L1.Features.Partners.GetBalance;
using L1.Features.Partners.GetBetLimit;
using L1.Features.Partners.Log;
using L1.Features.Partners.PlaceBet;
using L1.Features.Partners.VoidGame;
using L1.Shared.Constants;

namespace L1.Features.SboCommunicators
{
    public class SboCustomerService : IPartnerCustomerServices
    {
        private readonly string PartnerId = PartnerIds.SBO;

        private readonly ISboService SboService;
        private readonly ILogService logService;
        private readonly SboEndPoints endpoints;

        public SboCustomerService(
            ISboService SboService,
            ILogService logService,
            SboEndPoints endpoints)
        {
            this.SboService = SboService;
            this.logService = logService;
            this.endpoints = endpoints;
        }

        public Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        => SboService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => SboService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);

        public Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        => SboService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub)
        {
            var response = await SboService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(PartnerId, response, system);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await SboService.Post<PartnerEndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(PartnerId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await SboService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(PartnerId, response);
            }

            return response;
        }
    }
}