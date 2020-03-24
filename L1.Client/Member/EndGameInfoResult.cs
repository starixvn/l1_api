namespace L1.Client
{
    public class EndGameInfoResult : Result
    {
        public EndGameInfoResult(EndGameInfoData data) : base(null)
        {
            Data = data;
        }

        public EndGameInfoResult(Error error) : base(error)
        {
        }

        public EndGameInfoData Data { get; }
    }
}