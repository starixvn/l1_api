using System;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.Resources;
using L1.Features.Partners;
using L1.Shared.Constants;
using L1.Shared.Resources;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.OWCommunicators
{
    public interface IOWFailureHandler
    {
        Task Handle<T>(OWResponse<T> response) where T : IPartnerResult;
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

        public async Task Handle<T>(OWResponse<T> response) where T : IPartnerResult
        {
            MapErrorMessage(response.Result);
            //await logService.LogFailedRequestAsync(response);

            sentryClient.CaptureMessage(
                $"Request to OW failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == OWSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(response.Request);
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = OWErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? Errors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}