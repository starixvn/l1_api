using L1.Features.PartnerCommunicators;
using System;

namespace L1.Features.PFFCommunicators
{
    public class PFFRequest
    {
        public PFFRequest(IPartnerCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IPartnerCall Call { get; }
    }
}