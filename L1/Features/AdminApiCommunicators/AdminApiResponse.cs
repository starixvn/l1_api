using System;

namespace L1.Features.AdminApiCommunicators
{
    public class AdminApiResponse<T> where T : AdminApiResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }
    }
}