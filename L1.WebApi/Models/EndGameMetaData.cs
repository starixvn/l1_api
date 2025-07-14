using System.Collections.Generic;

namespace L1.WebApi.Models
{
    public class EndGameMetaData
    {
        public List<BetDetail> BetDetails { get; set; }

        public string Result { get; set; }
    }
}
