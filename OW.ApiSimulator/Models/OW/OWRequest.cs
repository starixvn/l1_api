using System;

namespace OW.ApiSimulator.Models.OW
{
    public class OWRequest<T> where T: OWCall
    {
        public DateTimeOffset TimeStamp { get; set; }

        public OWAuth Auth { get; set; }

        public T Call { get; set; }
    }
}
