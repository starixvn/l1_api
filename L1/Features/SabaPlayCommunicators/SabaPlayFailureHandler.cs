using System;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators.Resources;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.SabaPlayCommunicators.CheckMaintenance;
using L1.Shared.Constants;
using Sentry;
using Sentry.Protocol;
using L1.Shared.Configurations;
using L1.Features.SboCommunicators;

namespace L1.Features.SabaPlayCommunicators
{
    public interface ISabaPlayFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult;
    }

    public class SabaPlayFailureHandler : ISabaPlayFailureHandler
    {
        private readonly ILogService logService;
        private readonly ISabaPlayMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public SabaPlayFailureHandler(ILogService logService, ISabaPlayMaintenanceService maintenanceService, ISentryClient sentryClient, IAppSettings appSettings)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public async Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(appSettings.PartnerIds.SabaPlay, response);

            sentryClient.CaptureMessage(
                $"Request to {appSettings.PartnerIds.SabaPlay} failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == PartnerSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(new SabaPlayRequest(response.Request.Call));
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}