using System;
using L1.Shared.Models;

namespace L1.Features.MemberAuthentications
{
    public class SingleSignOnResult : BaseResult
    {
        public SingleSignOnResult(int code, string message, Uri redirectUri = null)
        {
            Code = code;
            Message = message;
            RedirectUri = redirectUri;
        }

        public static SingleSignOnResult ServerErrorResult => new SingleSignOnResult(ServerErrorCode, string.Empty);

        public Uri RedirectUri { get; }

        public static SingleSignOnResult CreateSuccessResponse(string redirectUrl)
            => new SingleSignOnResult(SuccessCode, string.Empty, new Uri(redirectUrl));

        public static SingleSignOnResult CreateErrorResponse(int code)
            => new SingleSignOnResult(code, string.Empty);
    }
}