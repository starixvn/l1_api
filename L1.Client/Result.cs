namespace L1.Client
{
    public abstract class Result
    {
        public Result(Error error)
        {
            Error = error;
        }

        public Error Error { get; }

        public bool IsSuccessful() => Error == null;
    }
}