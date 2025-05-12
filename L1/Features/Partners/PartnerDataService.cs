using System;
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
        
        Task<PromotionDto> GetPromotion(string partnerId, string promotionCode);
    }

    public class PartnerDataService : IPartnerDataService
    {
        private readonly IPartnerDataAccess partnerDataAccess;
        private readonly ICurrencyDataAccess currencyDataAccess;
        private readonly IPromotionDataAccess promotionDataAccess;

        public PartnerDataService(
            IPartnerDataAccess partnerDataAccess, 
            ICurrencyDataAccess currencyDataAccess,
            IPromotionDataAccess promotionDataAccess)
        {
            this.partnerDataAccess = partnerDataAccess;
            this.currencyDataAccess = currencyDataAccess;
            this.promotionDataAccess = promotionDataAccess;
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
            
        public async Task<PromotionDto> GetPromotion(string partnerId, string promotionCode)
        {
            var promotion = await promotionDataAccess.GetPromotion(partnerId, promotionCode);
            if (promotion == null)
            {
                return null;
            }
            
            var currentDateTime = DateTime.UtcNow;
            var status = DeterminePromotionStatus(promotion.StartTime, promotion.EndTime, currentDateTime);
            
            return new PromotionDto
            {
                PromotionCode = promotion.PromotionCode,
                PromotionName = promotion.PromotionName,
                Status = status,
                StartTime = promotion.StartTime,
                EndTime = promotion.EndTime
            };
        }
        
        private PromotionStatus DeterminePromotionStatus(DateTime startTime, DateTime endTime, DateTime currentTime)
        {
            return currentTime switch
            {
                var time when time < startTime => PromotionStatus.Upcoming,
                var time when time > endTime => PromotionStatus.Completed,
                _ => PromotionStatus.Running
            };
        }
    }
}