using L1.Features.Partners;
using System;

namespace L1.Features.SboCommunicators
{
    public class SboRequest
    {
        public SboRequest(IPartnerCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IPartnerCall Call { get; }
    }
}