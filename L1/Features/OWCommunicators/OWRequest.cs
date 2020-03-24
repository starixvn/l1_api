using System;

namespace L1.Features.OWCommunicators
{
    public class OWRequest
    {
        public OWRequest(IOWAuth auth, IOWCall call)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Auth = auth;
            Call = call ?? throw new ArgumentNullException(nameof(call));
        }

        public DateTimeOffset TimeStamp { get; }

        public IOWAuth Auth { get; }

        public IOWCall Call { get; }
    }
}