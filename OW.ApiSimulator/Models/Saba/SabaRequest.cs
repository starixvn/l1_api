using System;

namespace OW.ApiSimulator.Models.Saba
{
    public class SabaRequest
    {
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;

        public string Seq { get; set; } = Guid.NewGuid().ToString();

        public Auth Auth { get; set; } = new Auth();
    }
}