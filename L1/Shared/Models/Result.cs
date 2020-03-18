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
        public const int ClientIdqDoesNotExistCode = 1;
        public const int WrongClientSecretCode = 2;
        public const int ApiServerNotExistCode = 3;
        public const int IdentityServerNotExistCode = 4;
        public const int ServerErrorCode = 5;
        public const int GameServerNotFoundCode = 6;

        public int Code { get; protected set; }

        public string Message { get; protected set; }

        public bool IsSuccessful => Code == SuccessCode;
    }
}