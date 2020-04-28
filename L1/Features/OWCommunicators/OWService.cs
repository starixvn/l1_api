using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace L1.Features.OWCommunicators
{
    public interface IOWService
    {
        Task<OWResponse<T>> Post<T>(string apiEndpoint, OWCall call) where T : IOWResult, new();
    }

    public class OWService : IOWService
    {
        private readonly HttpClient httpClient;
        private readonly OWAuth auth;
        private readonly IOWFailureHandler failureHandler;

        public OWService(HttpClient httpClient, OWAuth auth, IOWFailureHandler failureHandler)
        {
            this.httpClient = httpClient;
            this.auth = auth;
            this.failureHandler = failureHandler;
        }

        public async Task<OWResponse<T>> Post<T>(string apiEndpoint, OWCall call) where T : IOWResult, new()
        {
            const string defaultMediaType = "application/json";

            var request = new OWRequest(auth, call);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, defaultMediaType);
            var response = await httpClient.PostAsync(apiEndpoint, content);
            OWResponse<T> finalResponse;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                finalResponse = JsonConvert.DeserializeObject<OWResponse<T>>(responseContent);
            }
            else
            {
                finalResponse = await CreateOWNetworkErrorResponse<T>(call, response);
            }

            finalResponse.Request = request;

            if (finalResponse.Result.IsFailed())
            {
                await failureHandler.Handle<T>(finalResponse);
            }

            return finalResponse;
        }

        private static async Task<OWResponse<T>> CreateOWNetworkErrorResponse<T>(OWCall call, HttpResponseMessage response) where T : IOWResult, new()
            => new OWResponse<T>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = new T
                {
                    ErrorCode = OWSpecialErrorCodes.NetworkError,
                    ErrorDescription = await GetErrorResponseContent(response),
                    Seq = call.Seq
                }
            };

        private static async Task<string> GetErrorResponseContent(HttpResponseMessage response)
            => $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}";
    }
}