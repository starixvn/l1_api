using L1.Shared.Constants;

namespace L1.Features.ChatbotApiCommunicators
{
    public class ChatbotApiResult
    {
        public string ErrorCode { get; set; } = ErrorCodes.SuccessCode;

        public string ErrorDescription { get; set; }
    }
}