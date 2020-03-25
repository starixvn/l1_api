namespace L1.Features.GameServerCommunicators
{
    public abstract class GameServerResult
    {
        public string Seq { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }
    }
}