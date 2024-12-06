using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.OWCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Shared.Constants;
using L1.Shared.Helpers;
using Newtonsoft.Json;

namespace L1.Features.SKCommunicators
{
    public interface ISKService
    {
        Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call, string operatorId) where T : PartnerResult, new();
    }

    public class SKService : ISKService
    {
        private const string HeaderAccessKey = "Saba-access-key";
        private readonly HttpClient httpClient;
        private readonly ISKFailureHandler failureHandler;
        private readonly string secretKey;

        public SKService(HttpClient httpClient, ISKFailureHandler failureHandler, SKServiceSettings skServiceSettings)
        {
            this.httpClient = httpClient;
            this.failureHandler = failureHandler;
            secretKey = skServiceSettings.SecretKey;
        }

        public async Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call, string operatorId) where T : PartnerResult, new()
        {
            const string defaultMediaType = "application/json";
            var request = new PartnerRequest(call);
            var requestBody = JsonConvert.SerializeObject(call);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            PartnerResponse<T> finalResponse;
            var accessKey = Md5Helper.Create($"{secretKey}{requestBody}");

            httpClient.DefaultRequestHeaders.Add(HeaderAccessKey, accessKey);
            var response = await httpClient.PostAsync(new Uri(apiEndpoint, UriKind.Relative), content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                finalResponse = JsonConvert.DeserializeObject<PartnerResponse<T>>(responseContent);
            }
            else
            {
                finalResponse = await CreateSKNetworkErrorResponse<T>(new OWCall(call.Seq, call.PlayerId, call.SiteId), response, accessKey);
            }

            finalResponse.Request = request;

            if (finalResponse.Result.IsFailed())
            {
                finalResponse.AccessKey = accessKey;
                await failureHandler.Handle<T>(finalResponse, operatorId);

                finalResponse.Result.ErrorCode = ErrorCodes.CovertErrorCode(finalResponse.Result.ErrorCode);
            }

            return finalResponse;
        }

        private static async Task<PartnerResponse<T>> CreateSKNetworkErrorResponse<T>(OWCall call, HttpResponseMessage response, string accessKey) where T : IPartnerResult, new()
            => new PartnerResponse<T>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = new T
                {
                    ErrorCode = PartnerSpecialErrorCodes.NetworkError,
                    ErrorDescription = await GetErrorResponseContent(response),
                    Seq = call.Seq
                },
                AccessKey = accessKey
            };

        private static async Task<string> GetErrorResponseContent(HttpResponseMessage response)
            => $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}";
    }
}