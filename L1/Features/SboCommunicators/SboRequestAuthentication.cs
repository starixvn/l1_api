using L1.Features.PartnerCommunicators;
using L1.Features.Partners;

namespace L1.Features.SboCommunicators
{
    public class SboRequestAuthentication : PartnerRequestAuthentication
    {
        public SboRequestAuthentication(IPartnerDataService partnerDataService, SboServiceSettings serviceSettings)
            : base(partnerDataService, serviceSettings.HMACKey)
        {
        }
    }
}