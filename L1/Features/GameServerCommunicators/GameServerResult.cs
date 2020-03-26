namespace L1.Features.GameServerCommunicators
{
    public abstract class GameServerResult
    {
        public string ErrorCode { get; set; }

        public string ErrorDescription { get; set; }
    }
}