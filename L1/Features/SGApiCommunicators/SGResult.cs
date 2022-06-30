using L1.Shared.Constants;

namespace L1.Features.SGApiCommunicators
{
    public class SGResult
    {
        public string ErrorCode { get; set; } = ErrorCodes.SuccessCode;

        public string ErrorDescription { get; set; }

        public string Seq { get; set; }
    }
}