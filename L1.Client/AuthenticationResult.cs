using System;

namespace L1.Client
{
    public interface IResult
    {
        int Code { get; }

        string Message { get; }
    }

    public class SingleSignOnResult : IResult
    {
        public const int SuccessCode = 0;
        public const int FailCode = 1;

        public SingleSignOnResult(int code, string message, Uri redirectUri = null)
        {
            Code = code;
            Message = message;
            RedirectUri = redirectUri;
        }

        public int Code { get; }

        public string Message { get; }

        public Uri RedirectUri { get; }

        public bool IsSuccessful() => Code == SuccessCode;

        public static SingleSignOnResult CreateSuccessResponse(string redirectUrl)
            => new SingleSignOnResult(SuccessCode, string.Empty, new Uri(redirectUrl));

        public static SingleSignOnResult CreateErrorResponse(string message)
            => new SingleSignOnResult(FailCode, message);
    }
}