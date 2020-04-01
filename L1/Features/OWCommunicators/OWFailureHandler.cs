using System;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.CheckMaintenance;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.Resources;
using L1.Shared.Constants;
using L1.Shared.Resources;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.OWCommunicators
{
    public interface IOWFailureHandler
    {
        Task Handle<T>(OWResponse<T> response) where T : IOWResult;
    }

    public class OWFailureHandler : IOWFailureHandler
    {
        private readonly ILogService logService;
        private readonly IMaintenanceService maintenanceService;

        public OWFailureHandler(ILogService logService, IMaintenanceService maintenanceService)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
        }

        public async Task Handle<T>(OWResponse<T> response) where T : IOWResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(response);

            SentrySdk.CaptureMessage(
                $"Request to OW failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);

            if (response.Result.ErrorCode == OWSpecialErrorCodes.UnderMaintenance)
            {
                await maintenanceService.ObserveMaintenanceSchedule(response.Request);
            }
        }

        private static void MapErrorMessage(IOWResult result)
            => result.ErrorMessage = OWErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? Errors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}