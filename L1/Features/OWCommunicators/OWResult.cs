namespace L1.Features.OWCommunicators
{
    public interface IOWResult
    {
        string Seq { get; set; }

        int ErrorCode { get; set; }

        string ErrorDescription { get; set; }
    }

    public class OWResult : IOWResult
    {
        public string Seq { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorDescription { get; set; }
    }
}