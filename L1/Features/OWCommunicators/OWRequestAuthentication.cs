using L1.Features.PartnerCommunicators;
using L1.Features.Partners;

namespace L1.Features.OWCommunicators
{
    public class OWRequestAuthentication : PartnerRequestAuthentication
    {
        public OWRequestAuthentication(IPartnerDataService partnerDataService, OWServiceSettings serviceSettings)
            : base(partnerDataService, serviceSettings.HMACKey)
        {
        }
    }
}