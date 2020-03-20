using System;

namespace OW.ApiSimulator.Models.OW
{
    public class OWResponse<T> where T: OWResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }
    }
}
