using System;

namespace L1.Features.OWCommunicators
{
    public class OWResponse<T> where T : IOWResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }

        internal OWRequest Request { get; set; }
    }
}