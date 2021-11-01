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
            RedirectUrl = RedirectUri == null ? string.Empty : RedirectUri.ToString();
        }

        public static SingleSignOnResult ServerErrorResult => new(ServerErrorCode, string.Empty);

        public Uri RedirectUri { get; }

        public string RedirectUrl { get; set; }

        public static SingleSignOnResult CreateSuccessResponse(string redirectUrl)
            => new(SuccessCode, string.Empty, new Uri(redirectUrl));

        public static SingleSignOnResult CreateErrorResponse(int code)
            => new(code, string.Empty);
    }
}