using L1.Shared.Constants;
using L1.Shared.Models;

namespace L1.Features.AdminApiCommunicators
{
    public class AdminApiResult : IResult
    {
        public string ErrorCode { get; set; } = ErrorCodes.SuccessCode;

        public string ErrorDescription { get; set; }

        public int Code { get; set; }

        public string Message => ErrorDescription;
    }
}