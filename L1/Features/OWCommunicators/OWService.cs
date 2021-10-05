using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using L1.Shared.Configurations;
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
        private readonly IOWFailureHandler failureHandler;
        private readonly string secretKey;

        public OWService(HttpClient httpClient, IOWFailureHandler failureHandler, IAppSettings appSettings)
        {
            this.httpClient = httpClient;
            this.failureHandler = failureHandler;
            secretKey = appSettings.OWServiceSettings.SecretKey;
        }

        public async Task<OWResponse<T>> Post<T>(string apiEndpoint, OWCall call) where T : IOWResult, new()
        {
            const string defaultMediaType = "application/json";
            var request = new OWRequest(call);
            var requestBody = JsonConvert.SerializeObject(call);
            var content = new StringContent(requestBody, Encoding.UTF8, defaultMediaType);

            OWResponse<T> finalResponse;
            var accessKey = CreateMD5($"{secretKey}{requestBody}");

            httpClient.DefaultRequestHeaders.Add("Saba-access-key", accessKey);
            var response = await httpClient.PostAsync(new Uri(apiEndpoint, UriKind.Relative), content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                finalResponse = JsonConvert.DeserializeObject<OWResponse<T>>(responseContent);
            }
            else
            {
                finalResponse = await CreateOWNetworkErrorResponse<T>(call, response, accessKey);
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

        private static async Task<OWResponse<T>> CreateOWNetworkErrorResponse<T>(OWCall call, HttpResponseMessage response, string accessKey) where T : IOWResult, new()
            => new OWResponse<T>
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Result = new T
                {
                    ErrorCode = OWSpecialErrorCodes.NetworkError,
                    ErrorDescription = await GetErrorResponseContent(response),
                    Seq = call.Seq
                },
                AccessKey = accessKey
            };

        private static async Task<string> GetErrorResponseContent(HttpResponseMessage response)
            => $"{response.StatusCode}-{response.ReasonPhrase}-{await response.Content.ReadAsStringAsync()}";
    }
}