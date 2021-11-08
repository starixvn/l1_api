using System;

namespace L1.Features.PartnerCommunicators
{
    public class PartnerResponse<T> where T : IPartnerResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }

        internal PartnerRequest Request { get; set; }

        internal string AccessKey { get; set; }
    }
}