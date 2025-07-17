using System.Collections.Generic;

namespace L1.WebApi.Models
{
    public class EndGameMetaData
    {
        public List<BetDetail> BetDetails { get; set; }

        public string Result { get; set; }

        public string GuaranteedWinStatus { get; set; }

        public bool GuaranteedWinTriggered { get; set; }

        public string GuaranteedLossStatus { get; set; }

        public bool GuaranteedLossTriggered { get; set; }
    }
}
