using L1.Features.PartnerCommunicators;

namespace L1.Features.SGCommunicators
{
    public class SGResult : IPartnerResult
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