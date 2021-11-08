using System.Threading.Tasks;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Shared.Constants;

namespace L1.Features.PartnerCommunicators
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