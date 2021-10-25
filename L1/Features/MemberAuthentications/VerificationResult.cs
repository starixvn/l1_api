using L1.Shared.Models;

namespace L1.Features.MemberAuthentications
{
    public class VerificationResult : BaseResult
    {
        public const int TokenNotFoundCode = 100;
        public const int TokenExpiredCode = 101;
        public const int InvalidUserAgentOrIPCode = 103;

        public VerificationResult(int code, string message)
        {
            Code = code;
            Message = message;
        }

#pragma warning disable S107 // Methods should not have too many parameters

        public VerificationResult(
            int code,
            string message,
            int memberId,
            string memberName,
            string siteId,
            string language,
            string currency,
            string seq,
            string partnerId,
            string domain,
            string siteName,
            byte appType
            )
#pragma warning restore S107 // Methods should not have too many parameters
            : this(code, message)
        {
            MemberId = memberId;
            MemberName = memberName;
            SiteId = siteId;
            Language = language;
            Currency = currency;
            Seq = seq;
            PartnerId = partnerId;
            Domain = domain;
            SiteName = siteName;
            AppType = appType;
        }

        public int MemberId { get; }

        public string MemberName { get; }

        public string SiteId { get; }

        public string PartnerId { get; }

        public string Domain { get; }

        public string Language { get; }

        public string Currency { get; }

        public string Seq { get; }

        public string MemberKey => $"{PartnerId}_{MemberId}";

        public string SiteName { get; set; }
        
        public byte AppType { get; set; }

        public static VerificationResult CreateErrorResponse(int code)
           => new VerificationResult(code, string.Empty);

#pragma warning disable S107 // Methods should not have too many parameters

        public static VerificationResult CreateSuccessResponse(
            int memberId,
            string memberName,
            string siteId,
            string language,
            string currency,
            string seq,
            string partnerId,
            string domain,
            string siteName,
            byte appType)
#pragma warning restore S107 // Methods should not have too many parameters
            => new VerificationResult(
                SuccessCode,
                string.Empty,
                memberId,
                memberName,
                siteId,
                language,
                currency,
                seq,
                partnerId,
                domain,
                siteName,
                appType);
    }
}