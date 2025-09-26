using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using L1.Features.SigningCredentials;
using Microsoft.IdentityModel.Tokens;

namespace L1.IdentityServerApi.IdentityStore
{
    public class SigningCredentialStore : ISigningCredentialStore, IValidationKeysStore
    {
        private readonly ISigningCredentialsDataAccess dataAccess;

        public SigningCredentialStore(ISigningCredentialsDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            var key = await this.dataAccess.GetMainKeyAsync();
            var rsa = RSA.Create();
            
            // Try PKCS#8 format first (what your key actually is), then fallback to PKCS#1
            try
            {
                rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(key.PrivateKey), out _);
                Console.WriteLine($"SUCCESS: PKCS#8 import - Key ID: {key.KeyId}, RSA Key Size: {rsa.KeySize} bits");
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine($"FAILED: PKCS#8 import failed - {ex.Message}");
                
                // Fallback to PKCS#1 format if PKCS#8 fails
                try
                {
                    rsa = RSA.Create(); // Reset the RSA object
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(key.PrivateKey), out _);
                    Console.WriteLine($"SUCCESS: PKCS#1 import - Key ID: {key.KeyId}, RSA Key Size: {rsa.KeySize} bits");
                }
                catch (CryptographicException ex2)
                {
                    Console.WriteLine($"FAILED: PKCS#1 import also failed - {ex2.Message}");
                    Console.WriteLine($"Key ID: {key.KeyId}, RSA Key Size: {rsa.KeySize} bits (default)");
                }
            }
            
            return new SigningCredentials(new RsaSecurityKey(rsa)
            {
                KeyId = key.KeyId
            }, SecurityAlgorithms.RsaSha256);
        }

        public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var keys = await this.dataAccess.GetAllAsync();

            return keys.Select(x =>
            {
                var rsa = RSA.Create();
                
                // Try PKCS#8 format first (what your key actually is), then fallback to PKCS#1
                try
                {
                    rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(x.PrivateKey), out _);
                }
                catch (CryptographicException)
                {
                    // Fallback to PKCS#1 format if PKCS#8 fails
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(x.PrivateKey), out _);
                }
                
                return new SecurityKeyInfo
                {
                    Key = new RsaSecurityKey(rsa)
                    {
                        KeyId = x.KeyId
                    },
                    SigningAlgorithm = SecurityAlgorithms.RsaSha256
                };
            });
        }
    }
}