using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using L1.Features.ChatbotApiCommunicators.ForwardMessage;
using L1.Features.ChatbotApiCommunicators.NotifyHighStake;
using Sentry;
using Sentry.Protocol;

namespace L1.Features.ChatbotApiCommunicators
{
    public interface IChatbotApiService
    {
        void ForwardMessage(ForwardMessageRequest request);

        void NotifyHighStake(NotifyHighStakeRequest request);
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

        public void NotifyHighStake(NotifyHighStakeRequest request)
        {
            if (request.Amount >= chatbotApiSettings.HighStake)
            {
                var conversationSetting = chatbotApiSettings.Conversations.HighStake;
                var message = string.Format(conversationSetting.MessageTemplate, request.GameTypeId);

                ForwardMessage(new ForwardMessageRequest(conversationSetting.ConversationId, message));
            }
        }

        public void ForwardMessage(ForwardMessageRequest request)
        {
            var parameters = new Dictionary<string, string> {
                { "conversationId", request.ConversationId },
                { "message", request.Message }
            };

            PostAndForget(chatbotApiSettings.Endpoints.ForwardMessage, parameters);
        }

        private void PostAndForget(string apiEndpoint, Dictionary<string, string> parameters)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await Post(apiEndpoint, parameters);
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureMessage(ex.ToString(), SentryLevel.Error);
                }
            });
        }

        private Task<HttpResponseMessage> Post(string apiEndpoint, Dictionary<string, string> parameters)
        {
            parameters.Add("token", chatbotApiSettings.Token);
            var content = new FormUrlEncodedContent(parameters);

            return httpClient.PostAsync(apiEndpoint, content);
        }
    }
}