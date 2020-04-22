using L1.Shared.Constants;

namespace L1.Features.GameServerCommunicators
{
    public class GameServerResult
    {
        public string ErrorCode { get; set; } = ErrorCodes.SuccessCode;

        public string ErrorDescription { get; set; }
    }
}