using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.Resources;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using L1.Shared.Resources;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.OWCommunicators
{
    public interface IOWFailureHandler
    {
        Task Handle<T>(OWRequest request, OWResponse<T> response) where T : IOWResult;
    }

    public class OWFailureHandler : IOWFailureHandler
    {
        private readonly HttpClient httpClient;
        private readonly OWEndPoints endpoints;
        private readonly ILogService logService;

        public OWFailureHandler(HttpClient httpClient, OWEndPoints endpoints, ILogService logService)
        {
            this.logService = logService;
            this.httpClient = httpClient;
            this.endpoints = endpoints;
        }

        public async Task Handle<T>(OWRequest request, OWResponse<T> response) where T : IOWResult
        {
            MapErrorMessage(response.Result);
            await logService.LogFailedRequestAsync(response);

            SentrySdk.CaptureMessage($"Request to OW failed at ${DateTimeOffset.UtcNow} - {response.Result.ErrorDescription}", SentryLevel.Warning);

            if (response.Result.ErrorCode == OWSpecialErrorCodes.UnderMaintenance)
            {
                await ScheduleMaintenance(request);
            }
        }

        private async Task ScheduleMaintenance(OWRequest request)
        {
            const string defaultMediaType = "application/json";

            var checkingMaintenanceCall = new OWCall(request.Call.ObCustId, request.Call.SiteId);
            var owRequestBody = new OWRequest(null, checkingMaintenanceCall);
            var content = new StringContent(JsonConvert.SerializeObject(owRequestBody), Encoding.UTF8, defaultMediaType);
            var response = await httpClient.PostAsync(endpoints.CheckMaintenance, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var maintenanceSchedule = JsonConvert.DeserializeObject<OWResponse<OWMaintenanceResult>>(responseContent);

                // TODO: Trigger maintenance plan at Game Admin.
            }
        }

        private static void MapErrorMessage(IOWResult result)
            => result.ErrorMessage = OWErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? Errors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}