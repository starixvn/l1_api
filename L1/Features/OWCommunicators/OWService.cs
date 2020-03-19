using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.Resources;
using L1.Shared.Constants;
using L1.Shared.Resources;
using Newtonsoft.Json;

namespace L1.Features.OWCommunicators
{
    public interface IOWService
    {
        Task<OWResponse<T>> Post<T>(string apiEndpoint, OWCall call) where T : IOWResult, new();
    }

    public class OWService : IOWService
    {
        private const string DefaultMediaType = "application/json";

        private readonly HttpClient _httpClient;
        private readonly OWAuth _auth;

        public OWService(HttpClient httpClient, OWAuth auth)
        {
            _httpClient = httpClient;
            _auth = auth;
        }

        public async Task<OWResponse<T>> Post<T>(string apiEndpoint, OWCall call) where T : IOWResult, new()
        {
            var owRequestBody = new OWRequest(_auth, call);
            var content = new StringContent(JsonConvert.SerializeObject(owRequestBody), Encoding.UTF8, DefaultMediaType);
            var response = await _httpClient.PostAsync(apiEndpoint, content);
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

            if (finalResponse.Result.IsFailed())
            {
                MapErrorMessage(finalResponse.Result);
            }

            return finalResponse;
        }

        private static async Task<OWResponse<T>> CreateOWNetworkErrorResponse<T>(OWCall call, HttpResponseMessage response) where T : IOWResult, new()
            => new OWResponse<T>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = new T
                {
                    ErrorCode = 5001,
                    ErrorDescription = await GetErrorResponseContent(response),
                    Seq = call.Seq
                }
            };

        private static async Task<string> GetErrorResponseContent(HttpResponseMessage response)
            => $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}";

        private static void MapErrorMessage(IOWResult result)
            => result.ErrorMessage = OWErrors.ResourceManager.GetString(result.ErrorCode.ToString()) ?? Errors.ResourceManager.GetString(ErrorCodes.InternalError);
    }
}