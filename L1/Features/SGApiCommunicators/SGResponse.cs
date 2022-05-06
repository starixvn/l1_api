using System;

namespace L1.Features.SGApiCommunicators
{
    public class SGResponse<T> where T : SGResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }
    }
}