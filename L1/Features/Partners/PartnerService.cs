using System.Threading.Tasks;

namespace L1.Features.Partners
{
    public interface IPartnerService
    {
        Task<bool> Verify(string partnerId, string secretKey, string currency);
    }

    public class PartnerService : IPartnerService
    {
        private readonly IPartnerDataAccess partnerDataAccess;
        private readonly ICurrencyDataAccess currencyDataAccess;

        public PartnerService(IPartnerDataAccess partnerDataAccess, ICurrencyDataAccess currencyDataAccess)
        {
            this.partnerDataAccess = partnerDataAccess;
            this.currencyDataAccess = currencyDataAccess;
        }

        public async Task<bool> Verify(string partnerId, string secretKey, string currency)
        {
            var isValidPartner = await partnerDataAccess.Verify(partnerId, secretKey);

            return isValidPartner ? await currencyDataAccess.Verify(partnerId, currency) : false;
        }
    }
}