using L1.Features.Partners;

namespace L1.Features.OWCommunicators
{
    public class OWResult : IPartnerResult
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