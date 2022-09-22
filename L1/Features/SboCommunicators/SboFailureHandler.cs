using System;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators.Resources;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.SboCommunicators.CheckMaintenance;
using L1.Shared.Constants;
using Sentry;
using Sentry.Protocol;
using L1.Shared.Configurations;

namespace L1.Features.SboCommunicators
{
    public interface ISboFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult;
    }

    public class SboFailureHandler : ISboFailureHandler
    {
        private readonly ILogService logService;
        private readonly ISboMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public SboFailureHandler(ILogService logService, ISboMaintenanceService maintenanceService, ISentryClient sentryClient, IAppSettings appSettings)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public async Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(appSettings.PartnerIds.SBO, response);

            sentryClient.CaptureMessage(
                $"Request to {appSettings.PartnerIds.SBO} failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
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