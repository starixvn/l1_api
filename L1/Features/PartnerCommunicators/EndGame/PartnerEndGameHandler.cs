using L1.Features.AdminApiCommunicators.VoidTicket;
using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators;
using System.Threading.Tasks;
using System;
using L1.Shared.Configurations;
using System.Linq;

public interface IPartnerEndGameHandler
{
    Task HandleFailedEndGame(PartnerEndGameCall call, PartnerResponse<PartnerEndGameResult> response, string operatorId);
}

public class PartnerEndGameHandler : IPartnerEndGameHandler
{
    private readonly IAdminApiService adminApiService;
    private readonly IAppSettings appSettings;

    public PartnerEndGameHandler(IAdminApiService adminApiService, IAppSettings appSettings)
    {
        this.adminApiService = adminApiService;
        this.appSettings = appSettings;
    }

    public async Task HandleFailedEndGame(PartnerEndGameCall call, PartnerResponse<PartnerEndGameResult> response, string operatorId)
    {
        if (!ShouldHandleFailedEndGame(operatorId))
        {
            return;
        }
        var voidTicketRequest = CreateVoidTicketRequest(call, response, operatorId);
        await adminApiService.VoidTicket(voidTicketRequest);
    }

    private bool ShouldHandleFailedEndGame(string operatorId)
    {
        return appSettings.VoidTicketWhenEndingRoundFailed?.Operators
            ?.Contains(operatorId, StringComparer.OrdinalIgnoreCase) ?? false;
    }

    private VoidTicketRequest CreateVoidTicketRequest(
        PartnerEndGameCall call,
        PartnerResponse<PartnerEndGameResult> response,
        string operatorId)
    {
        return new VoidTicketRequest(
            playerId: call.PlayerId,
            operatorId: operatorId,
            siteId: call.SiteId,
            gameRoundId: call.GameRoundId,
            gameTypeId: call.GameTypeId,
            reason: response.Result.ErrorMessage);
    }
}