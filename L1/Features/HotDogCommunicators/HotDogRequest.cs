using L1.Features.PartnerCommunicators;
using System;

namespace L1.Features.HotdogCommunicators
{
    public class HotdogRequest
    {
        public HotdogRequest(IPartnerCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IPartnerCall Call { get; }
    }
}