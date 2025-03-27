using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.SGCommunicators
{
    public class SGResponse<T> where T : PartnerResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }

        internal SGRequest Request { get; set; }

        internal string AccessKey { get; set; }
    }
} 