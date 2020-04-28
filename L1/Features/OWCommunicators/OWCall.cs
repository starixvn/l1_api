using System;

namespace L1.Features.OWCommunicators
{
    public interface IOWCall
    {
        string Seq { get; set; }

        int ObCustId { get; set; }

        string SiteId { get; set; }
    }

    public class OWCall : IOWCall
    {
        public OWCall(int obCustId, string siteId)
            : this(Guid.NewGuid().ToString(), obCustId, siteId)
        {
        }

        public OWCall(string seq, int obCustId, string siteId)
        {
            Seq = seq ?? throw new ArgumentNullException(nameof(seq));
            SiteId = siteId ?? throw new ArgumentNullException(nameof(siteId));
            ObCustId = obCustId;
        }

        public string Seq { get; set; }

        public int ObCustId { get; set; }

        public string SiteId { get; set; }
    }
}