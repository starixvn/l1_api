namespace L1.Features.ChatbotApiCommunicators.ForwardMessage
{
    public class ForwardMessageRequest : ChatbotApiRequest
    {
        public ForwardMessageRequest(string conversationId, string message)
        {
            ConversationId = conversationId;
            Message = message;
        }

        public string ConversationId { get; set; }

        public string Message { get; set; }
    }
}