namespace L1.Features.OneTwoCheersCommunicators
{
    using System;
    using L1.Features.PartnerCommunicators;
    public class OneTwoCheersRequest
    {
        public OneTwoCheersRequest(IPartnerCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IPartnerCall Call { get; }
    }
}
