using System;
using L1.Features.Partners;

namespace L1.Features.OWCommunicators
{
    public class OWResponse<T> where T : IPartnerResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }

        internal OWRequest Request { get; set; }

        internal string AccessKey { get; set; }
    }
}