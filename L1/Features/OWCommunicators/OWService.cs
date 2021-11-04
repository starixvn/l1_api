using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Features.Partners;
using Newtonsoft.Json;

namespace L1.Features.OWCommunicators
{
    public interface IOWService
    {
        Task<PartnerResponse<T>> Post<T>(string apiEndpoint, IPartnerCall call) where T : PartnerResult, new();
    }

    public class OWService : IOWService
    {
        private readonly HttpClient httpClient;
        private readonly IOWFailureHandler failureHandler;
        private readonly string secretKey;

        public OWService(HttpClient httpClient, IOWFailureHandler failureHandler, OWServiceSettings owServiceSettings)
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
            var accessKey = CreateMD5($"{secretKey}{requestBody}");

            httpClient.DefaultRequestHeaders.Add("Saba-access-key", accessKey);
            var response = await httpClient.PostAsync(new Uri(apiEndpoint, UriKind.Relative), content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                finalResponse = JsonConvert.DeserializeObject<PartnerResponse<T>>(responseContent);
            }
            else
            {
                finalResponse = await CreateOWNetworkErrorResponse<T>(new OWCall(call.Seq, call.ObCustId, call.SiteId), response, accessKey);
            }

            finalResponse.Request = request;

            if (finalResponse.Result.IsFailed())
            {
                finalResponse.AccessKey = accessKey;
                await failureHandler.Handle<T>(finalResponse);
            }

            return finalResponse;
        }

        private static string CreateMD5(string input)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private static async Task<PartnerResponse<T>> CreateOWNetworkErrorResponse<T>(OWCall call, HttpResponseMessage response, string accessKey) where T : IPartnerResult, new()
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