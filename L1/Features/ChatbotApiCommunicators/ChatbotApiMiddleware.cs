using Microsoft.Extensions.DependencyInjection;

namespace L1.Features.ChatbotApiCommunicators
{
    public static class ChatbotApiMiddleware
    {
        public static void AddChatbotApiServices(this IServiceCollection services, ChatbotApiSettings chatbotApiSettings)
        {
            services.AddSingleton(chatbotApiSettings.Endpoints);
            services
                .AddHttpClient<IChatbotApiService, ChatbotApiService>(httpClient =>
                {
                    httpClient.BaseAddress = chatbotApiSettings.BaseUrl;
                });
        }
    }
}