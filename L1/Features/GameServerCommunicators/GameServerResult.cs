namespace L1.Features.GameServerCommunicators
{
    public abstract class GameServerResult
    {
        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }
    }
}