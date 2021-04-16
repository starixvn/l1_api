using System;

namespace L1.Features.ChatbotApiCommunicators
{
    public class ChatbotApiResponse<T> where T : ChatbotApiResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }
    }
}