using System;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.Resources;
using L1.Shared.Constants;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.OWCommunicators
{
    public interface IOWFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult;
    }

    public class OWFailureHandler : IOWFailureHandler
    {
        private readonly ILogService logService;
        private readonly IOWMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;
        private readonly string PartnerId = PartnerIds.OW;

        public OWFailureHandler(ILogService logService, IOWMaintenanceService maintenanceService, ISentryClient sentryClient)
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
                await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(response.Request.Call));
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}