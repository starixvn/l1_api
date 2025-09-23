namespace L1.Features.OneTwoCheersCommunicators
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using L1.Features.PartnerCommunicators;
    using L1.Shared.Constants;
    using L1.Shared.Helpers;
    using L1.Shared.Json;
    public interface IOneTwoCheersService
    {
        Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call, string operatorId) where T : PartnerResult, new();
    } 
    public class OneTwoCheersService : IOneTwoCheersService
    {
        private const string HeaderAccessKey = "Saba-access-key";
        private readonly string secretKey;
        private readonly IJsonService jsonService;
        private readonly HttpClient httpClient;
        private readonly IOneTwoCheersFailureHandler failureHandler;

        public OneTwoCheersService(
            OneTwoCheersServiceSettings owServiceSettings,
            IJsonService jsonService,
            HttpClient httpClient,
            IOneTwoCheersFailureHandler failureHandler)
        {
            this.secretKey = owServiceSettings.SecretKey;
            this.jsonService = jsonService;
            this.httpClient = httpClient;
            this.failureHandler = failureHandler;
        }

        public async Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call, string operatorId) where T : PartnerResult, new()
        {
            var request = new PartnerRequest(call);
            var requestBody = jsonService.Serialize(call);

            using var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var accessKey = Md5Helper.Create($"{secretKey}{requestBody}");

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
                    : await CreateOWNetworkErrorResponse<T>(new OneTwoCheersCall(call.Seq, call.PlayerId, call.SiteId), response, accessKey);

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
                return new PartnerResponse<T>
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
            }
        }

        private static async Task<PartnerResponse<T>> CreateOWNetworkErrorResponse<T>(OneTwoCheersCall call, HttpResponseMessage response, string accessKey) where T : IPartnerResult, new()
        {
            var errorContent = await GetErrorResponseContent(response);
            return new PartnerResponse<T>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = new T
                {
                    ErrorCode = PartnerSpecialErrorCodes.NetworkError,
                    ErrorDescription = errorContent,
                    Seq = call.Seq
                },
                AccessKey = accessKey
            };
        }

        private static async Task<string> GetErrorResponseContent(HttpResponseMessage response)
            => $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}";
    }
}
