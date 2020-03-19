using System;

namespace OW.SimulatorApi.Models.OW
{
    public class OWRequest<T> where T: OWCall
    {
        public DateTimeOffset TimeStamp { get; set; }

        public OWAuth Auth { get; set; }

        public T Call { get; set; }
    }
}
