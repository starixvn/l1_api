using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.PartnerCommunicators;
using Newtonsoft.Json;

namespace L1.Features.SabaPlayCommunicators.DailySummary
{
    public interface ISabaPlayDailySummaryService
    {
        Task<DailySummaryResult> Post();
    }

    public class SabaPlayDailySummaryService : ISabaPlayDailySummaryService
    {
        private readonly HttpClient httpClient;
        private readonly ISabaPlayFailureHandler failureHandler;
        private readonly SabaPlayDailySummaryServiceSettings settings;

        public SabaPlayDailySummaryService(HttpClient httpClient, ISabaPlayFailureHandler failureHandler, SabaPlayDailySummaryServiceSettings settings)
        {
            this.httpClient = httpClient;
            this.failureHandler = failureHandler;
            this.settings = settings;
        }

        public async Task<DailySummaryResult> Post()
        {
            const string defaultMediaType = "application/json";
            var call = new DailySummaryCall();
            var requestBody = JsonConvert.SerializeObject(call);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            var finalResponse = new DailySummaryResponse { TimeStamp = DateTimeOffset.UtcNow };

            var httpResponse = await httpClient.PostAsync(new Uri(settings.Endpoints.DailySummary, UriKind.Relative), content);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                finalResponse.Result = JsonConvert.DeserializeObject<DailySummaryResult>(responseContent);
            }
            else
            {
                finalResponse.Result = await CreateNetworkErrorResult(call, httpResponse);
            }

            finalResponse.Request = new PartnerRequest(call);

            if (finalResponse.Result.IsFailed())
            {
                await failureHandler.Handle<DailySummaryResult>(finalResponse);
            }

            return finalResponse.Result;
        }

        private static async Task<DailySummaryResult> CreateNetworkErrorResult(DailySummaryCall request, HttpResponseMessage response)
            => new()
            {
                TimeStamp = DateTimeOffset.UtcNow,
                ErrorCode = PartnerSpecialErrorCodes.NetworkError,
                ErrorDescription = await GetErrorResponseContent(response),
                Seq = request.Seq
            };

        private static async Task<string> GetErrorResponseContent(HttpResponseMessage response)
            => $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}";
    }
}