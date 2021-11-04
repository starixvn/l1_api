using System;
using L1.Features.Partners;

namespace L1.Features.SboCommunicators
{
    public class SboResponse<T> where T : PartnerResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }

        internal SboRequest Request { get; set; }

        internal string AccessKey { get; set; }
    }
}