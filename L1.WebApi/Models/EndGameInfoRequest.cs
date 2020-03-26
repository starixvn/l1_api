using System;

namespace L1.WebApi.Models
{
    public class EndGameInfoRequest
    {
        public DateTimeOffset TimeStamp { get; set; }

        public int GameRoundId { get; set; }

        public int GameTypeId { get; set; }

        public int ObCustId { get; set; }

        public string SiteId { get; set; }

        public string Seq { get; set; }

        public Auth Auth { get; set; }
    }
}