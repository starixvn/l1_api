using System.Threading.Tasks;
using L1.Features.Partners.EndGame;
using L1.Features.Partners.EnterPortal;
using L1.Features.Partners.GetBalance;
using L1.Features.Partners.GetBetLimit;
using L1.Features.Partners.PlaceBet;
using L1.Features.Partners.VoidGame;
using L1.Shared.Constants;

namespace L1.Features.Partners
{
    public interface IPartnerCustomerServices
    {
        Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call);

        Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call);

        Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call);

        Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub);

        Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call);

        Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call);
    }
}