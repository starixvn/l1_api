using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.Resources;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using L1.Shared.Resources;
using Newtonsoft.Json;

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
        private readonly ILogDataAccess logDataAccess;

        public OWFailureHandler(HttpClient httpClient, OWEndPoints endpoints, ILogDataAccess logDataAccess)
        {
            this.logDataAccess = logDataAccess;
            this.httpClient = httpClient;
            this.endpoints = endpoints;
        }

        public async Task Handle<T>(OWRequest request, OWResponse<T> response) where T : IOWResult
        {
            MapErrorMessage(response.Result);
            await WriteFailedRequestLog(request, response);

            if (response.Result.ErrorCode == OWSpecialErrorCodes.UnderMaintenance)
            {
                await ScheduleMaintenance(request);
            }
        }

        private Task WriteFailedRequestLog<T>(OWRequest request, OWResponse<T> response) where T : IOWResult
        {
            var log = new OWLog(request.TimeStamp, request.Call, response.TimeStamp, response.Result);

            return logDataAccess.WriteAsync(log);
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