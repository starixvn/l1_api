using System;

namespace L1.WebApi.Models
{
    public class AddSpins
    {
        public DateTimeOffset TimeStamp { get; set; }

        public string Key { get; set; }

        public int ObCustId { get; set; }

        public byte Spins { get; set; }

        public string Seq { get; set; }

        public Auth Auth { get; set; }
    }
}
