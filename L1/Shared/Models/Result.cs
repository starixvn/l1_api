using System;

namespace L1.Shared.Models
{
    public interface IResult
    {
        int Code { get; }

        string Message { get; }
    }

    public abstract class BaseResult : IResult
    {
        public const int SuccessCode = 0;
        public const int ClientIdDoesNotExistCode = 1;
        public const int WrongClientSecretCode = 2;
        public const int ApiServerNotExistCode = 3;
        public const int IdentityServerNotExistCode = 4;
        public const int ServerErrorCode = 5;
        public const int GameServerNotFoundCode = 6;

        public int Code { get; protected set; }

        public string Message { get; protected set; }

        public bool IsSuccessful => Code == SuccessCode;
    }

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

        public VerificationResult(int code, string message, int memberId, string memberName, string siteId, string language)
            : this(code, message)
        {
            MemberId = memberId;
            MemberName = memberName;
            SiteId = siteId;
            Language = language;
            MemberBalance = 10000000;
        }

        public int MemberId { get; }

        public string MemberName { get; }

        public string SiteId { get; }

        public string Language { get; }

        public decimal MemberBalance { get; } 

        public string MemberKey => $"{SiteId}_{MemberId}";

        public static VerificationResult CreateErrorResponse(int code)
           => new VerificationResult(code, string.Empty);

        public static VerificationResult CreateSuccessResponse(int memberId, string memberName, string siteId, string language)
            => new VerificationResult(SuccessCode, string.Empty, memberId, memberName, siteId, language);
    }

    public class SingleSignOnResult : BaseResult
    {
        public SingleSignOnResult(int code, string message, Uri redirectUri = null)
        {
            Code = code;
            Message = message;
            RedirectUri = redirectUri;
        }

        public Uri RedirectUri { get; }

        public static SingleSignOnResult CreateSuccessResponse(string redirectUrl)
            => new SingleSignOnResult(SuccessCode, string.Empty, new Uri(redirectUrl));

        public static SingleSignOnResult CreateErrorResponse(int code)
            => new SingleSignOnResult(code, string.Empty);

        public static SingleSignOnResult ServerErrorResult => new SingleSignOnResult(ServerErrorCode, string.Empty);
    }
}