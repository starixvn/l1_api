using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using L1.Features.Partners;
using L1.Shared.Constants;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.SGApiCommunicators
{
    public interface ISGApiService
    {
        Task<SGResponse<DailySummaryResult>> DailySummary(DailySummaryRequest request);
        Task<SGResponse<RetrieveEndRoundInfoResult>> RetrieveEndRoundInfoAsync(RetrieveEndRoundInfoRequest request);
        Task<SGResponse<T>> Post<T>(string apiEndpoint, SGRequest request) where T : SGResult, new();
    }

    public class SGApiService : ISGApiService
    {
        private readonly HttpClient httpClient;
        private readonly SGApiEndPoints sgApiEndPoints;
        private readonly ISentryClient sentryClient;
        private readonly IPartnerDataService partnerDataService;

        public SGApiService(HttpClient httpClient, SGApiEndPoints sgApiEndPoints, ISentryClient sentryClient, IPartnerDataService partnerDataService)
        {
            this.httpClient = httpClient;
            this.sgApiEndPoints = sgApiEndPoints;
            this.sentryClient = sentryClient;
            this.partnerDataService = partnerDataService;
        }

        public Task<SGResponse<RetrieveEndRoundInfoResult>> RetrieveEndRoundInfoAsync(RetrieveEndRoundInfoRequest request)
            => Post<RetrieveEndRoundInfoResult>(sgApiEndPoints.RetrieveEndRoundInfo, request);

        public Task<SGResponse<DailySummaryResult>> DailySummary(DailySummaryRequest request)
            => Post<DailySummaryResult>(sgApiEndPoints.DailySummary, request);

        public async Task<SGResponse<T>> Post<T>(string apiEndpoint, SGRequest request) where T : SGResult, new()
        {
            var @operator = await partnerDataService.GetById(request.Auth.OperatorId);
            var requestBody = JsonConvert.SerializeObject(request);
            httpClient.DefaultRequestHeaders.Add("Access-Key", CreateAccessKey(@operator.SecretKey, requestBody));

            var response = await Post(apiEndpoint, request);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<SGResponse<T>>(responseContent);

            }

            var failureResponse = await CreateErrorResponse<T>(response);

            sentryClient.CaptureMessage(
                $"Request to SG API's {apiEndpoint} failed at {DateTimeOffset.UtcNow} - {failureResponse.Result.ErrorDescription}",
                SentryLevel.Warning);

            return failureResponse;
        }

        private Task<HttpResponseMessage> Post(string apiEndpoint, SGRequest request)
        {
            const string defaultMediaType = "application/json";

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, defaultMediaType);

            return httpClient.PostAsync(new Uri(apiEndpoint, UriKind.Relative), content);
        }

        private static async Task<SGResponse<T>> CreateErrorResponse<T>(HttpResponseMessage response) where T : SGResult, new()
          => new SGResponse<T>
          {
              TimeStamp = DateTimeOffset.UtcNow,
              Result = new T
              {
                  ErrorCode = ErrorCodes.InternalError,
                  ErrorDescription = $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}"
              }
          };

        private string CreateAccessKey(string secretKey, string data)
        {
            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secretKey));
            var hashResult = hmac.ComputeHash(Encoding.ASCII.GetBytes(data));

            return Convert.ToBase64String(hashResult);
        }
    }
}