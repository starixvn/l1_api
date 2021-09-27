using System.Collections.Generic;
using System.Linq;
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

        private SigningCredentialKey GetValidKey()
            => new SigningCredentialKey
            {
                Id = "api",
                PrivateKey = "MIIEowIBAAKCAQEA8mIiMGsruz6/qxzTkYg5iVKL6G0fG29o8xPY3F3i4GuhkqMS3ZrFnau/aQBB/SPmLqxmD7d1DwS+eo1Esd61q8zVXhJUC6jaTcHc4J4KC145+BJ8cvXNKXCw8THKB5nJ8/1V6v1ykR9SPwGDaJNV7o0fyQyUbVX8BzXmsr2IWyMKr+vu41PiFXE4OIhxv2ptYSYtnoK8k6FB3rSLv05Ci3Uyn9fPbrL5rfNB7LZssMO6Cvb9jHYyHhGBdn5u+0WBxA8Sz7gfqJIqJK0h2SsRDKCv9GcJ9F6QuYt6hCw0k4imdvEx3pcApdbAkGZGPzy7iUQuTgp05JKmB0S3fUQe+wIDAQABAoIBAHf6C2JcKjFtRgK1KiSHgbNvoMFUAmnaI+DumhG6h2r+2+NxxQzcZ6U8NmsLLZ7X/kBsJbjx3oeomYCgHLjOvK5lndgKyn9a/XQv2d11XRNzeM5KmD9vXDZ/CZbaRl6syUJ0y+qdufALyPJXFWzWQVrrkCg8idv1C+Ol0qP3HV++agGkZM+Zt0pu5c8xZ/VbVUPBMrVrmfbY7vUDjaxLntXA0UGevX3X2vdE4OD6kUFrYSNpLLbfqEAmkw7jzPvY64adhLtZdoyCAIOzS8mJUyY8lKHc+3o+nMr0aXkZI/VaGqOZwnnTAGOupgbxgC6h4IIQdIf/QApgNuD06ageq4ECgYEA//SC5Z8VaMNiuWPTrX6N7+ZJfNcqiutvL52yquEK7h4HAq4jvDW8pVB18z+YHoqELMP6eLBa3HE6gHZNDQfQrdtj+6ld2U8Gt1aUP0wqIkb52OcliQOQ8jwVug3Ip61GlKTcFOGIVr42zoMpOp4zd3ZYwmponwjPfGU4VfgFeokCgYEA8m0DV8VscVVKZ92tUuDPx6rKlXQWOdhHfYgCzd3fvvu7iB7HlwF9fReaiUeTNT50T1q6e2O+KKnhxi6KbmQJgigbkKwEZn/8216jpyssA8xoshGZZ4ErYKvqYnqI8p3slstrPYlLEECFq0zAvtwMXbzM0EPxEM+TbMvP0RkY3GMCgYAd5YqQladjiEb0zqNABaNAnU6CJYPTnDJthmcDLKwhKiK5fmuwJTq2uJI+anQzRQyxSebsx/SlARjLPoY7emk2lYS1NZA/vMpVCejhB914CYBPhJhOy7rXgIstI0UHyOaQoD0Qt1M8hpVSQ+HB9g6e6pTtiI5V3SqXJh+Rcxn4uQKBgFGwsmOtPzGhkAUYCgPXhay+vK1IBQVRACYo4ukETxVINpVT6eXUtrikb4q697wFY6Qp6pURs054kH/7xuIUC7A0W7WQgZsYndg+N99pCNfu1YjLrWnXeQqGY+DzcZpv5P2bZnPCA+1iEKj8oCawd+pOaKX1fxIvIHqU416OmnYJAoGBANfIzcYI+OcSrx0ULWitt9NmnxzP3mFQwzbiUHTR6YSy21673U8KhpdJor/9exzHaVVAkCAnS1gr0LcTOQZCNMLllfcDUmVlb7qCZiTT8iGGJJ7D8nLeQL+j5GECeUeqcblK1kdfJ0uJnBQpsr1eOE0fo9CLSdg9itE2SE7I2iYD",
                IsMain = true,
                KeyId = "302dd617-fcd5-408d-a31a-7d49c0fa7cd2"
            };
    }
}