using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using L1.Features.ChatbotApiCommunicators.ForwardMessage;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.ChatbotApiCommunicators
{
    public interface IChatbotApiService
    {
        Task ForwardMessage(ForwardMessageRequest request);
    }

    public class ChatbotApiService : IChatbotApiService
    {
        private readonly HttpClient httpClient;
        private readonly ChatbotApiSettings chatbotApiSettings;

        public ChatbotApiService(HttpClient httpClient, ChatbotApiSettings chatbotApiSettings)
        {
            this.httpClient = httpClient;
            this.chatbotApiSettings = chatbotApiSettings;
        }

        public Task ForwardMessage(ForwardMessageRequest request)
        {
            var parameters = new Dictionary<string, string> {
                { "conversationId", request.ConversationId },
                { "message", request.Message }
            };

            return PostAndForget(chatbotApiSettings.Endpoints.FowardMessage, parameters);
        }

        private async Task PostAndForget(string apiEndpoint, Dictionary<string, string> parameters)
        {
            try
            {
                await Post(apiEndpoint, parameters);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage(ex.ToString(), SentryLevel.Error);
            }
        }

        private Task<HttpResponseMessage> Post(string apiEndpoint, Dictionary<string, string> parameters)
        {
            parameters.Add("token", chatbotApiSettings.Token);
            var content = new FormUrlEncodedContent(parameters);

            return httpClient.PostAsync(apiEndpoint, content);
        }
    }
}