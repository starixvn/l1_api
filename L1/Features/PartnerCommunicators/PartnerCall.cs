using System;

namespace L1.Features.PartnerCommunicators
{
    public interface IPartnerCall
    {
        string Seq { get; set; }

        string PlayerId { get; set; }

        string SiteId { get; set; }
    }

    public class PartnerCall : IPartnerCall
    {
        public PartnerCall(string playerId, string siteId)
            : this(Guid.NewGuid().ToString(), playerId, siteId)
        {
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public PartnerCall(string seq, string playerId, string siteId)
        {
            TimeStamp = DateTimeOffset.UtcNow;
            Seq = seq;
            SiteId = siteId ?? throw new ArgumentNullException(nameof(siteId));
            PlayerId = playerId;
        }

        public string Seq { get; set; }

        public string PlayerId { get; set; }

        public string SiteId { get; set; }

        public DateTimeOffset TimeStamp { get; }
    }
}