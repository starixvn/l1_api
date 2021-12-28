using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using L1.Features.Partners;
using NSubstitute;
using Xunit;

namespace L1.Tests.Features.Partners
{
    public class PartnerDataServiceTests
    {
        private readonly IPartnerDataAccess partnerDataAccess;
        private readonly ICurrencyDataAccess currencyDataAccess;
        private readonly Fixture fixture = new();

        private readonly PartnerDataService partnerDataService;

        public PartnerDataServiceTests()
        {
            partnerDataAccess = Substitute.For<IPartnerDataAccess>();
            currencyDataAccess = Substitute.For<ICurrencyDataAccess>();

            partnerDataService = new PartnerDataService(partnerDataAccess, currencyDataAccess);
        }

        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        [Theory]
        public async Task GetById_IdIsNullOrWhiteSpaces_ReturnNull(string id)
        {
            // Arrange

            // Act
            var result = await partnerDataService.GetById(id);

            // Assert
            Assert.Null(result);
            await partnerDataAccess.DidNotReceive().GetById(id);
        }

        [Theory, AutoData]
        public async Task GetById_IdNotNull_ReturnCorrectValue(string id)
        {
            // Arrange
            var partner = fixture.Create<Partner>();
            partnerDataAccess.GetById(id).Returns(partner);

            // Act
            var result = await partnerDataService.GetById(id);

            // Assert
            Assert.Equal(partner, result);
            await partnerDataAccess.Received(1).GetById(id);
        }

        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("   ", "   ")]
        [Theory]
        public async Task GetCurrency_PartnerIdIsNullOrWhiteSpaces_ReturnNull(string partnerId, string currencyName)
        {
            // Arrange

            // Act
            var result = await partnerDataService.GetCurrency(partnerId, currencyName);

            // Assert
            Assert.Null(result);
            await currencyDataAccess.DidNotReceive().Get(partnerId, currencyName);
        }

        [Theory, AutoData]
        public async Task GetCurrency_PartnerIdNotNull_ReturnCorrectValue(string partnerId, string currencyName)
        {
            // Arrange
            var currency = fixture.Create<Currency>();
            currencyDataAccess.Get(partnerId, currencyName).Returns(currency);

            // Act
            var result = await partnerDataService.GetCurrency(partnerId, currencyName);

            // Assert
            Assert.Equal(currency, result);
            await currencyDataAccess.Received(1).Get(partnerId, currencyName);
        }

        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("   ", "   ")]
        [Theory]
        public async Task Verify_PartnerIdIsNullOrWhiteSpaces_ReturnFalse(string partnerId, string secretKey)
        {
            // Arrange

            // Act
            var result = await partnerDataService.Verify(partnerId, secretKey);

            // Assert
            Assert.False(result);
            await partnerDataAccess.DidNotReceive().Verify(partnerId, secretKey);
        }

        [Theory, AutoData]
        public async Task Verify_PartnerIdNotNull_ReturnCorrectValue(string partnerId, string secretKey)
        {
            // Arrange
            var isVerify = fixture.Create<bool>();
            partnerDataAccess.Verify(partnerId, secretKey).Returns(isVerify);

            // Act
            var result = await partnerDataService.Verify(partnerId, secretKey);

            // Assert
            Assert.Equal(isVerify, result);
            await partnerDataAccess.Received(1).Verify(partnerId, secretKey);
        }

        [InlineData("", "", "")]
        [InlineData(null, null, null)]
        [InlineData("   ", "   ", "   ")]
        [Theory]
        public async Task VerifyWithCurrency_PartnerIdIsNullOrWhiteSpaces_ReturnFalse(string partnerId, string secretKey, string currency)
        {
            // Arrange

            // Act
            var result = await partnerDataService.Verify(partnerId, secretKey, currency);

            // Assert
            Assert.False(result);
            await partnerDataAccess.DidNotReceive().Verify(partnerId, secretKey);
        }

        [Theory, AutoData]
        public async Task VerifyWithCurrency_PartnerIdNotNull_ReturnCorrectValue(string partnerId, string secretKey, string currency)
        {
            // Arrange
            var isVerifiedPartner = fixture.Create<bool>();
            var isVerifiedCurrency = fixture.Create<bool>();
            partnerDataAccess.Verify(partnerId, secretKey).Returns(isVerifiedPartner);
            currencyDataAccess.Verify(partnerId, currency).Returns(isVerifiedCurrency);

            // Act
            var result = await partnerDataService.Verify(partnerId, secretKey, currency);

            // Assert
            Assert.Equal(isVerifiedPartner && isVerifiedCurrency, result);
            await partnerDataAccess.Received(1).Verify(partnerId, secretKey);
            await currencyDataAccess.Received(1).Verify(partnerId, currency);
        }
    }
}