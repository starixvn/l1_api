using System;

namespace L1.Features.ChatbotApiCommunicators
{
    public class ChatbotApiSettings
    {
        public string Token { get; set; }

        public Uri BaseUrl { get; set; }

        public ChatbotApiEndPoints Endpoints { get; set; }
    }

    public class ChatbotApiEndPoints
    {
        public string ForwardMessage { get; set; }
    }
}