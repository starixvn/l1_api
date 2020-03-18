using L1.Shared.Models;

namespace L1.Features.Authentications
{
    public class VerificationResult : BaseResult
    {
        public const int TokenNotFoundCode = 100;
        public const int TokenExpiredCode = 101;
        public const int UserDeviceNotFoundCode = 102;

        public VerificationResult(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public VerificationResult(
            int code,
            string message,
            int memberId,
            string memberName,
            string siteId,
            string language,
            string currency,
            string seq)
            : this(code, message)
        {
            MemberId = memberId;
            MemberName = memberName;
            SiteId = siteId;
            Language = language;
            MemberBalance = 10000000;
            Currency = currency;
            Seq = seq;
        }

        public int MemberId { get; }

        public string MemberName { get; }

        public string SiteId { get; }

        public string Language { get; }

        public decimal MemberBalance { get; }

        public string Currency { get; }

        public string Seq { get; }

        public string MemberKey => $"{SiteId}_{MemberId}";

        public static VerificationResult CreateErrorResponse(int code)
           => new VerificationResult(code, string.Empty);

        public static VerificationResult CreateSuccessResponse(
            int memberId,
            string memberName,
            string siteId,
            string language,
            string currency,
            string seq)
            => new VerificationResult(
                SuccessCode,
                string.Empty,
                memberId,
                memberName,
                siteId,
                language,
                currency,
                seq);
    }
}