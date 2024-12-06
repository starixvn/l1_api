using System;
using System.Threading.Tasks;
using L1.Features.SKCommunicators.CheckMaintenance;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.Resources;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using Sentry;
using Sentry.Protocol;
using L1.Features.OWCommunicators;

namespace L1.Features.SKCommunicators
{
    public interface ISKFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult;
    }

    public class SKFailureHandler : ISKFailureHandler
    {
        private readonly ILogService logService;
        private readonly ISKMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public SKFailureHandler(ILogService logService, ISKMaintenanceService maintenanceService, ISentryClient sentryClient, IAppSettings appSettings)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public async Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(operatorId, response);

            sentryClient.CaptureMessage(
                $"Request to {operatorId} failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == PartnerSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(response.Request.Call), operatorId);
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}