using System;
using System.Threading.Tasks;
using L1.Features.Partners;
using L1.Features.Partners.Log;
using L1.Features.Partners.Resources;
using L1.Features.SboCommunicators.CheckMaintenance;
using L1.Shared.Constants;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.SboCommunicators
{
    public interface ISboFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult;
    }

    public class SboFailureHandler : ISboFailureHandler
    {
        private readonly string PartnerId = PartnerIds.SBO;

        private readonly ILogService logService;
        private readonly IMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;

        public SboFailureHandler(ILogService logService, IMaintenanceService maintenanceService, ISentryClient sentryClient)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
        }

        public async Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(PartnerId, response);

            sentryClient.CaptureMessage(
                $"Request to {PartnerId} failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == PartnerSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(new SboRequest(response.Request.Call));
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}