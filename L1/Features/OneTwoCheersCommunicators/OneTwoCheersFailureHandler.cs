namespace L1.Features.OneTwoCheersCommunicators
{
    using System;
    using System.Threading.Tasks;
    using L1.Features.PartnerCommunicators.Log;
    using L1.Features.PartnerCommunicators.Resources;
    using L1.Features.PartnerCommunicators;
    using L1.Shared.Constants;
    using Sentry;
    using L1.Features.OneTwoCheersCommunicators.CheckMaintenance;

    public interface IOneTwoCheersFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult;
    }

    public class OneTwoCheersFailureHandler : IOneTwoCheersFailureHandler
    {
        private readonly ILogService logService;
        private readonly IOneTwoCheersMaintenanceService maintenanceService;
        private readonly ISentryClient sentryClient;

        public OneTwoCheersFailureHandler(IOneTwoCheersMaintenanceService maintenanceService, ISentryClient sentryClient, ILogService logService)
        {
            this.logService = logService;
            this.maintenanceService = maintenanceService;
            this.sentryClient = sentryClient;
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
                await maintenanceService.ObserveMaintenanceSchedule(new OneTwoCheersRequest(response.Request.Call), operatorId);
            }
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}
