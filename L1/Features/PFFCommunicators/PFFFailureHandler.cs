using System;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators.Resources;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PFFCommunicators.CheckMaintenance;
using L1.Shared.Constants;
using Sentry;
using Sentry.Protocol;
using L1.Shared.Configurations;
using L1.Features.SboCommunicators;

namespace L1.Features.PFFCommunicators
{
    public interface IPFFFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult;
    }

    public class PFFFailureHandler : IPFFFailureHandler
    {
        private readonly ILogService logService;
        private readonly IPFFMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public PFFFailureHandler(ILogService logService, IPFFMaintenanceService maintenanceService, ISentryClient sentryClient, IAppSettings appSettings)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public async Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(appSettings.PartnerIds.PFF, response);

            sentryClient.CaptureMessage(
                $"Request to {appSettings.PartnerIds.PFF} failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == PartnerSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(new PFFRequest(response.Request.Call));
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}