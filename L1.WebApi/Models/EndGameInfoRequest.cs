using System;
using L1.Shared.Models;

namespace L1.WebApi.Models
{
    public class EndGameInfoRequest : IRequest
    {
        public DateTimeOffset TimeStamp { get; set; }

        public int GameRoundId { get; set; }

        public byte GameTypeId { get; set; }

        public int ObCustId { get; set; }

        public string SiteId { get; set; }

        public string Seq { get; set; }

        public Auth Auth { get; set; }

        public bool FreeBet { get; set; }
    }
}