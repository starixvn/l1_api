using System;

namespace L1.Features.OWCommunicators
{
    public class OWRequest
    {
        public OWRequest(IOWCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IOWCall Call { get; }
    }
}