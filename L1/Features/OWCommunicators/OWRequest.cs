using System;
using L1.Features.Partners;

namespace L1.Features.OWCommunicators
{
    public class OWRequest
    {
        public OWRequest(IPartnerCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IPartnerCall Call { get; }
    }
}