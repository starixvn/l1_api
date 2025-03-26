using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.SboCommunicators
{
    public class SboCall : PartnerCall
    {
        public SboCall(object playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public SboCall(string seq, object playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}