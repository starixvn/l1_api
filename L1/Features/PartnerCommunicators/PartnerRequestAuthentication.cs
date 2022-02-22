using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace L1.Features.PartnerCommunicators
{
    public interface IPartnerRequestAuthentication
    {
        Task<bool> Verify(string secretKey, HttpRequest request);
    }

    public class PartnerRequestAuthentication : IPartnerRequestAuthentication
    {
        public async Task<bool> Verify(string secretKey, HttpRequest request)
        {
            var accessKey = request.Headers["Access-Key"];

            if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
            {
                return false;
            }

            request.Body.Seek(0, SeekOrigin.Begin);
            var streamData = new byte[request.Body.Length];
            await request.Body.ReadAsync(streamData.AsMemory(0, streamData.Length));

            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(secretKey));
            var hashResult = hmac.ComputeHash(streamData);

            return accessKey == Convert.ToBase64String(hashResult);
        }
    }
}