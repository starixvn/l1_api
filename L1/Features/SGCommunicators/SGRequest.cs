using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.SGCommunicators
{
    public class SGRequest
    {
        public SGRequest(IPartnerCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IPartnerCall Call { get; }
    }
} 