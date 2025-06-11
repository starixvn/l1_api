using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using L1.Shared.Constants;
using L1.Shared.Helpers;
using L1.Shared.Json;
using Sentry;

namespace L1.Features.HotdogCommunicators
{
    public interface IHotdogService
    {
        Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call, string operatorId) where T : PartnerResult, new();
    }

    public class HotdogService : IHotdogService
    {
        private const string HeaderAccessKey = "Access-Key";
        private readonly HttpClient httpClient;
        private readonly IHotdogFailureHandler failureHandler;
        private readonly string secretKey;
        private readonly IJsonService jsonService;
        private readonly ISentryClient sentryClient;

        public HotdogService(
            HttpClient httpClient,
            IHotdogFailureHandler failureHandler,
            HotdogServiceSettings hotdogServiceSettings,
            IJsonService jsonService,
            ISentryClient sentryClient)
        {
            this.httpClient = httpClient;
            this.failureHandler = failureHandler;
            this.secretKey = hotdogServiceSettings.SecretKey;
            this.jsonService = jsonService;
            this.sentryClient = sentryClient;
        }

        public async Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call, string operatorId) where T : PartnerResult, new()
        {
            var request = new PartnerRequest(call);
            var requestBody = jsonService.Serialize(call);

            using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(this.secretKey));
            var hashResult = hmac.ComputeHash(Encoding.ASCII.GetBytes(requestBody));
            var accessKey = Convert.ToBase64String(hashResult);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(apiEndpoint, UriKind.Relative))
            {
                Content = content
            };
            requestMessage.Headers.Add(HeaderAccessKey, accessKey);

            try
            {
                using var response = await httpClient.SendAsync(requestMessage);
                var finalResponse = response.IsSuccessStatusCode
                    ? jsonService.Deserialize<PartnerResponse<T>>(await response.Content.ReadAsStringAsync())
                    : await CreateHotdogNetworkErrorResponse<T>(new HotdogCall(call.Seq, call.PlayerId, call.SiteId), response, accessKey);

                finalResponse!.Request = request;

                if (finalResponse.Result.IsFailed())
                {
                    finalResponse.AccessKey = accessKey;
                    await failureHandler.Handle<T>(finalResponse, operatorId);
                    finalResponse.Result.ErrorCode = ErrorCodes.CovertErrorCode(finalResponse.Result.ErrorCode);
                }

                return finalResponse;
            }
            catch (HttpRequestException ex)
            {
                var operatorResponse = new PartnerResponse<T>
                {
                    TimeStamp = DateTimeOffset.UtcNow,
                    Request = request,
                    Result = new T
                    {
                        ErrorCode = PartnerSpecialErrorCodes.NetworkError,
                        ErrorDescription = ex.Message,
                        Seq = call.Seq
                    },
                    AccessKey = accessKey
                };

                sentryClient.CaptureException(new Exception(jsonService.Serialize(operatorResponse), ex));

                return operatorResponse;
            }
        }

        private static async Task<PartnerResponse<T>> CreateHotdogNetworkErrorResponse<T>(HotdogCall call, HttpResponseMessage response, string accessKey) where T : IPartnerResult, new()
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