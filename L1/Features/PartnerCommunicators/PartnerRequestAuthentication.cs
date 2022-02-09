using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using L1.Features.Partners;
using Microsoft.AspNetCore.Http;

namespace L1.Features.PartnerCommunicators
{
    public class PartnerRequestAuthentication
    {
        private readonly string secretKey;
        private readonly IPartnerDataService partnerDataService;

        protected PartnerRequestAuthentication(IPartnerDataService partnerDataService, string secretKey)
        {
            this.partnerDataService = partnerDataService;
            this.secretKey = secretKey;
        }

        public async Task<bool> Verify(string partnerId, string partnerSecret, HttpRequest request, string data)
        {
            var accessKey = request.Headers["access-key"];
            return VerifyAccessKey(accessKey, data) && await VerifyPartner(partnerId, partnerSecret);
        }

        private async Task<bool> VerifyPartner(string partnerId, string partnerSecret)
        {
            return await partnerDataService.Verify(partnerId, partnerSecret);
        }

        private bool VerifyAccessKey(string accessKey, string data)
        {
            if (string.IsNullOrWhiteSpace(accessKey))
            {
                return false;
            }

            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secretKey));

            var hashResult = hmac.ComputeHash(Encoding.ASCII.GetBytes(data));

            return accessKey == Convert.ToBase64String(hashResult);
        }
    }
}