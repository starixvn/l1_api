using System.Threading.Tasks;

namespace L1.Features.Partners
{
    public interface IPartnerDataService
    {
        Task<Partner> GetById(string id);

        Task<bool> Verify(string partnerId, string secretKey);

        Task<bool> Verify(string partnerId, string secretKey, string currency);

        Task<Currency> GetCurrency(string partnerId, string currencyName);
    }

    public class PartnerDataService : IPartnerDataService
    {
        private readonly IPartnerDataAccess partnerDataAccess;
        private readonly ICurrencyDataAccess currencyDataAccess;

        public PartnerDataService(IPartnerDataAccess partnerDataAccess, ICurrencyDataAccess currencyDataAccess)
        {
            this.partnerDataAccess = partnerDataAccess;
            this.currencyDataAccess = currencyDataAccess;
        }

        public Task<Partner> GetById(string id) => partnerDataAccess.GetById(id);

        public Task<Currency> GetCurrency(string partnerId, string currencyName)
        => currencyDataAccess.Get(partnerId, currencyName);

        public async Task<bool> Verify(string partnerId, string secretKey, string currency)
        {
            var isValidPartner = await partnerDataAccess.Verify(partnerId, secretKey);

            return isValidPartner ? await currencyDataAccess.Verify(partnerId, currency) : false;
        }

        public Task<bool> Verify(string partnerId, string secretKey) => partnerDataAccess.Verify(partnerId, secretKey);
    }
}