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
        public const int ServerErrorCode = 1;
        public const int InvalidDataErrorCode = 2;
        public const int UnauthorizedErrorCode = 3;
        public const int ServerNotFoundCode = 4;

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

        public static SingleSignOnResult CreateServerErrorResponse()
            => new SingleSignOnResult(ServerErrorCode, SingleSignOnResultMessage.ServerError);

        public static SingleSignOnResult CreateInvalidDataErrorResponse()
            => new SingleSignOnResult(InvalidDataErrorCode, SingleSignOnResultMessage.InvalidData);

        public static SingleSignOnResult CreateUnauthorizedErrorResponse()
            => new SingleSignOnResult(UnauthorizedErrorCode, SingleSignOnResultMessage.Unauthorized);

        public static SingleSignOnResult CreateServerNotFoundResponse()
           => new SingleSignOnResult(ServerNotFoundCode, SingleSignOnResultMessage.ServerNotFound);
    }
}