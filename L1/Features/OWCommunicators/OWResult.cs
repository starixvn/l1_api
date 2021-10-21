namespace L1.Features.OWCommunicators
{
    public interface IOWResult
    {
        string Seq { get; set; }

        int ErrorCode { get; set; }

        string ErrorMessage { get; set; }

        string ErrorDescription { get; set; }

        bool IsSuccessful();

        bool IsFailed();
    }

    public class OWResult : IOWResult
    {
        public string Seq { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorDescription { get; set; }

        public bool IsSuccessful()
            => !IsFailed();

        public bool IsFailed()
            => ErrorCode != 0;
    }
}