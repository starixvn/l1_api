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
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(key.PrivateKey), out _);

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
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(x.PrivateKey), out _);

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