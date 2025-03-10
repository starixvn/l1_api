using System;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.Resources;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using Sentry;

namespace L1.Features.OWCommunicators
{
    public interface IOWFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult;
    }

    public class OWFailureHandler : IOWFailureHandler
    {
        private readonly ILogService logService;
        private readonly IOWMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public OWFailureHandler(ILogService logService, IOWMaintenanceService maintenanceService, ISentryClient sentryClient, IAppSettings appSettings)
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