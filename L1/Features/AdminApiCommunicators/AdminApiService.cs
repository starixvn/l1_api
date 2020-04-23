using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators.UnderMaintenance;
using L1.Shared.Constants;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.AdminApiCommunicators
{
    public interface IAdminApiService
    {
        Task UnderMaintenance(MaintenanceRequest request);
    }

    public class AdminApiService : IAdminApiService
    {
        private readonly HttpClient httpClient;
        private readonly AdminApiEndPoints adminApiEndPoints;

        public AdminApiService(HttpClient httpClient, AdminApiEndPoints adminApiEndPoints)
        {
            this.httpClient = httpClient;
            this.adminApiEndPoints = adminApiEndPoints;
        }

        public Task UnderMaintenance(MaintenanceRequest request)
            => PostAndForget(adminApiEndPoints.UnderMaintenance, request);

        public async Task<AdminApiResponse<T>> Post<T>(string apiEndpoint, AdminApiRequest request) where T : AdminApiResult, new()
        {
            var response = await Post(apiEndpoint, request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                return new AdminApiResponse<T>
                {
                    TimeStamp = DateTimeOffset.UtcNow,
                    Result = JsonConvert.DeserializeObject<T>(responseContent)
                };
            }

            var failureResponse = await CreateErrorResponse<T>(response);

            SentrySdk.CaptureMessage(
                $"Request to Saba Admin API's {apiEndpoint} failed at {DateTimeOffset.UtcNow} - {failureResponse.Result.ErrorDescription}",
                SentryLevel.Warning);

            return failureResponse;
        }

        public async Task PostAndForget(string apiEndpoint, AdminApiRequest request)
        {
            try
            {
                await Post(apiEndpoint, request);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage(ex.ToString(), SentryLevel.Error);
            }
        }

        private Task<HttpResponseMessage> Post(string apiEndpoint, AdminApiRequest request)
        {
            const string defaultMediaType = "application/json";

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, defaultMediaType);

            return httpClient.PostAsync(apiEndpoint, content);
        }

        private static async Task<AdminApiResponse<T>> CreateErrorResponse<T>(HttpResponseMessage response) where T : AdminApiResult, new()
          => new AdminApiResponse<T>
          {
              TimeStamp = DateTimeOffset.UtcNow,
              Result = new T
              {
                  ErrorCode = ErrorCodes.InternalError,
                  ErrorDescription = $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}"
              }
          };
    }
}