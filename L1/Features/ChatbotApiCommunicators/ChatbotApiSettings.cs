using System;
using System.Collections.Generic;

namespace L1.Features.ChatbotApiCommunicators
{
    public class ChatbotApiSettings
    {
        public string Token { get; set; }

        public decimal HighStake { get; set; }

        public Uri BaseUrl { get; set; }

        public ChatbotApiEndPoints Endpoints { get; set; }

        public ConversationSettings Conversations { get; set; }
    }

    public class ChatbotApiEndPoints
    {
        public string ForwardMessage { get; set; }
    }

    public class ConversationSettings 
    {   
        public MessageSettings HighStake { get; set; }
    }

    public class MessageSettings
    {
        public string ConversationId { get; set; }

        public string MessageTemplate { get; set; }
    }
}