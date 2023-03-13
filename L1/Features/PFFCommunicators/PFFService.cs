using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using L1.Shared.Helpers;
using Newtonsoft.Json;

namespace L1.Features.PFFCommunicators
{
    public interface IPFFService
    {
        Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call) where T : PartnerResult, new();
    }

    public class PFFService : IPFFService
    {
        private readonly HttpClient httpClient;
        private readonly IPFFFailureHandler failureHandler;
        private readonly string secretKey;

        public PFFService(HttpClient httpClient, IPFFFailureHandler failureHandler, PFFServiceSettings owServiceSettings)
        {
            this.httpClient = httpClient;
            this.failureHandler = failureHandler;
            secretKey = owServiceSettings.SecretKey;
        }

        public async Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call) where T : PartnerResult, new()
        {
            const string defaultMediaType = "application/json";
            var request = new PartnerRequest(call);
            var requestBody = JsonConvert.SerializeObject(call);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            PartnerResponse<T> finalResponse;
            var accessKey = Md5Helper.Create($"{secretKey}{requestBody}");

            httpClient.DefaultRequestHeaders.Add("Saba-access-key", accessKey);
            var response = await httpClient.PostAsync(new Uri(apiEndpoint, UriKind.Relative), content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                finalResponse = JsonConvert.DeserializeObject<PartnerResponse<T>>(responseContent);
            }
            else
            {
                finalResponse = await CreateNetworkErrorResponse<T>(new PFFCall(call.Seq, call.PlayerId, call.SiteId), response, accessKey);
            }

            finalResponse.Request = request;

            if (finalResponse.Result.IsFailed())
            {
                finalResponse.AccessKey = accessKey;
                await failureHandler.Handle<T>(finalResponse);
            }

            return finalResponse;
        }

        private static async Task<PartnerResponse<T>> CreateNetworkErrorResponse<T>(PFFCall call, HttpResponseMessage response, string accessKey) where T : IPartnerResult, new()
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