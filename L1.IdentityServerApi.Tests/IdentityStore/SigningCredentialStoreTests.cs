using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using L1.Features.SigningCredentials;
using L1.Features.Sites;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Xunit;

namespace L1.IdentityServerApi.IdentityStore
{
    public class SigningCredentialStoreTests
    {
        private readonly ISigningCredentialsDataAccess dataAccess;
        private readonly SigningCredentialStore store;

        public SigningCredentialStoreTests()
        {
            dataAccess = Substitute.For<ISigningCredentialsDataAccess>();
            store = new SigningCredentialStore(dataAccess);
        }

        [Fact]
        public async Task GetSigningCredentialsAsync_Always_ReturnCorrectResult()
        {
            // Arrange
            var key = GetValidKey();
            dataAccess.GetMainKeyAsync().Returns(key);

            // Act
            var actualValue = await store.GetSigningCredentialsAsync();

            // Assert
            Assert.True(actualValue.Algorithm == SecurityAlgorithms.RsaSha256);
        }

        [Fact]
        public async Task GetValidationKeysAsync_Always_ReturnCorrectResult()
        {
            // Arrange
            var keys = new List<SigningCredentialKey> { GetValidKey() };
            dataAccess.GetAllAsync().Returns(keys);

            // Act
            var actualValue = await store.GetValidationKeysAsync();

            // Assert
            Assert.Single(actualValue);
        }

        private static SigningCredentialKey GetValidKey()
        {
            // Generate a proper PKCS#8 private key for testing
            using var rsa = RSA.Create(2048);
            var privateKeyBytes = rsa.ExportPkcs8PrivateKey();
            var privateKeyBase64 = Convert.ToBase64String(privateKeyBytes);
            
            return new SigningCredentialKey
            {
                Id = "api",
                PrivateKey = privateKeyBase64,
                IsMain = true,
                KeyId = "302dd617-fcd5-408d-a31a-7d49c0fa7cd2"
            };
        }
    }
}