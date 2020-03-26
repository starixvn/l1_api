using System;

namespace OW.ApiSimulator.Models.Saba
{
    public class EndGameInfoRequest
    {
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public int ObCustId { get; set; }

        public string SiteId { get; set; }

        public string Seq { get; set; } = Guid.NewGuid().ToString();

        public Auth Auth { get; set; } = new Auth();
    }
}