using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.SboCommunicators
{
    public class SboCall : PartnerCall
    {
        public SboCall(int playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public SboCall(string seq, int playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}