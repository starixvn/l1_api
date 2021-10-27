using System;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.OWCommunicators.Resources;
using L1.Features.Partners;
using L1.Features.Partners.Log;
using L1.Shared.Constants;
using L1.Shared.Resources;
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
        private readonly IMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;

        public OWFailureHandler(ILogService logService, IMaintenanceService maintenanceService, ISentryClient sentryClient)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
        }

        public async Task Handle<T>(PartnerResponse<T> response) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(response);

            sentryClient.CaptureMessage(
                $"Request to OW failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == OWSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(new OWRequest(response.Request.Call));
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = OWErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? Errors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}