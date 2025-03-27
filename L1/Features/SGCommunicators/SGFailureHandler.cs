using System;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.Resources;
using L1.Shared.Constants;
using Sentry;

namespace L1.Features.SGCommunicators
{
    public interface ISGFailureHandler
    {
        Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult;
    }

    public class SGFailureHandler : ISGFailureHandler
    {
        private readonly ILogService logService;
        private readonly ISentryClient sentryClient;

        public SGFailureHandler(ILogService logService, ISentryClient sentryClient)
        {
            this.logService = logService;
            this.sentryClient = sentryClient;
        }

        public async Task Handle<T>(PartnerResponse<T> response, string operatorId) where T : PartnerResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(operatorId, response);

            sentryClient.CaptureMessage(
                $"Request to {operatorId} failed at {DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}",
                SentryLevel.Warning);
        }

        private static void MapErrorMessage(IPartnerResult result)
            => result.ErrorMessage = PartnerErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? PartnerErrors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
} 