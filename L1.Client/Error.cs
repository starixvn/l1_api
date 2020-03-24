using System.Net;
using L1.Client.Resources;

namespace L1.Client
{
    public class Error
    {
        public const int ServerErrorCode = 1;
        public const int InvalidDataErrorCode = 2;
        public const int UnauthorizedErrorCode = 3;
        public const int ServerNotFoundCode = 4;

        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }

        public static Error CreateErrorFromResponseStatusCode(HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.Unauthorized)
            {
                return new Error(UnauthorizedErrorCode, ResultMessage.Unauthorized);
            }

            if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                return new Error(InvalidDataErrorCode, ResultMessage.InvalidData);
            }

            if (httpStatusCode == HttpStatusCode.NotFound)
            {
                return new Error(ServerNotFoundCode, ResultMessage.ServerNotFound);
            }

            return new Error(ServerErrorCode, ResultMessage.ServerError);
        }
    }
}