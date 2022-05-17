using System.Threading.Tasks;

namespace L1.Features.Partners
{
    public interface IPartnerDataService
    {
        Task<Partner> GetById(string id);

        Task<bool> Verify(string partnerId, string secretKey);

        Task<bool> Verify(string partnerId, string secretKey, string currency);

        Task<Currency> GetCurrency(string partnerId, string currencyName);

        Task<bool> VerifyWithCurrency(string partnerId, string currency);
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

        public Task<Partner> GetById(string id)
        => string.IsNullOrWhiteSpace(id) ? Task.FromResult<Partner>(default) : partnerDataAccess.GetById(id);

        public Task<Currency> GetCurrency(string partnerId, string currencyName)
        => string.IsNullOrWhiteSpace(partnerId) || string.IsNullOrWhiteSpace(currencyName)
            ? Task.FromResult<Currency>(default)
            : currencyDataAccess.Get(partnerId, currencyName);

        public async Task<bool> Verify(string partnerId, string secretKey, string currency)
        {
            var isValidPartner = await Verify(partnerId, secretKey);

            return isValidPartner && await currencyDataAccess.Verify(partnerId, currency);
        }

        public Task<bool> Verify(string partnerId, string secretKey)
        => string.IsNullOrWhiteSpace(partnerId) || string.IsNullOrWhiteSpace(secretKey)
            ? Task.FromResult(false)
            : partnerDataAccess.Verify(partnerId, secretKey);

        public Task<bool> VerifyWithCurrency(string partnerId, string currency)
            => currencyDataAccess.Verify(partnerId, currency);
    }
}