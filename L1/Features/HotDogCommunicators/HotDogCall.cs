using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.HotdogCommunicators
{
    public class HotdogCall : PartnerCall
    {
        public HotdogCall(object playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public HotdogCall(string seq, object playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}