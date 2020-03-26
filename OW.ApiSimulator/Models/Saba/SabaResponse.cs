using System;

namespace OW.ApiSimulator.Models.Saba
{
    public class SabaResponse<T> where T : SabaResult
    {
        public DateTimeOffset TimeStamp { get; set; }

        public T Result { get; set; }
    }
}