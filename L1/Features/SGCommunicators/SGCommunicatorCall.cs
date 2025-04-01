using System;
using L1.Features.PartnerCommunicators;
using Newtonsoft.Json;

namespace L1.Features.SGCommunicators
{
    public class SGCommunicatorCall : PartnerCall
    {
        public SGCommunicatorCall(int playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
        }

        public SGCommunicatorCall(string seq, int playerId, string siteId)
            : base(seq, playerId.ToString(), siteId)
        {
        }

        [JsonIgnore]
        public new int PlayerId
        {
            get => Convert.ToInt32(base.PlayerId);
            set => base.PlayerId = value.ToString();
        }
    }
}