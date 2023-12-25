using System;
using L1.Features.PartnerCommunicators;

namespace L1.Features.SabaPlayCommunicators
{
    public class SabaPlayCall : PartnerCall
    {
        public SabaPlayCall(string playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public SabaPlayCall(string seq, string playerId, string siteId) : base(seq, playerId, siteId)
        {
        }
    }
}