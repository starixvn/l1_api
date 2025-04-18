using System;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators.Resources;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.HotdogCommunicators.CheckMaintenance;
using L1.Shared.Constants;
using Sentry;
using L1.Shared.Configurations;

namespace L1.Features.HotdogCommunicators
{
    public interface IHotdogFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult;
    }

    public class HotdogFailureHandler : IHotdogFailureHandler
    {
        private readonly ILogService logService;
        private readonly IHotdogMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public HotdogFailureHandler(ILogService logService, IHotdogMaintenanceService maintenanceService, ISentryClient sentryClient, IAppSettings appSettings)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public async Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(appSettings.PartnerIds.SabaPlay_HD, response);

            sentryClient.CaptureMessage(
                $"Request to {appSettings.PartnerIds.SabaPlay_HD} failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == PartnerSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(new HotdogRequest(response.Request.Call), operatorId);
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}