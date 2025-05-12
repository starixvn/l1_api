using System;
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
        private readonly IPromotionDataAccess promotionDataAccess;
        private readonly Fixture fixture = new();

        private readonly PartnerDataService partnerDataService;

        public PartnerDataServiceTests()
        {
            partnerDataAccess = Substitute.For<IPartnerDataAccess>();
            currencyDataAccess = Substitute.For<ICurrencyDataAccess>();
            promotionDataAccess = Substitute.For<IPromotionDataAccess>();

            partnerDataService = new PartnerDataService(partnerDataAccess, currencyDataAccess, promotionDataAccess);
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

        [Fact]
        public async Task GetPromotion_WithValidData_ShouldReturnPromotionWithUpcomingStatus()
        {
            // Arrange
            var partnerId = "PARTNER123";
            var promotionCode = "PROMO123";
            var futureStartTime = DateTime.UtcNow.AddDays(1);
            var futureEndTime = DateTime.UtcNow.AddDays(10);
            
            var promotion = new Promotion
            {
                PartnerId = partnerId,
                PromotionCode = promotionCode,
                PromotionName = "Test Promotion",
                StartTime = futureStartTime,
                EndTime = futureEndTime
            };

            promotionDataAccess.GetPromotion(partnerId, promotionCode).Returns(promotion);

            // Act
            var result = await partnerDataService.GetPromotion(partnerId, promotionCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(promotionCode, result.PromotionCode);
            Assert.Equal("Test Promotion", result.PromotionName);
            Assert.Equal(PromotionStatus.Upcoming, result.Status);
            Assert.Equal(futureStartTime, result.StartTime);
            Assert.Equal(futureEndTime, result.EndTime);
            await promotionDataAccess.Received(1).GetPromotion(partnerId, promotionCode);
        }

        [Fact]
        public async Task GetPromotion_WithValidData_ShouldReturnPromotionWithRunningStatus()
        {
            // Arrange
            var partnerId = "PARTNER123";
            var promotionCode = "PROMO123";
            var pastStartTime = DateTime.UtcNow.AddDays(-1);
            var futureEndTime = DateTime.UtcNow.AddDays(1);
            
            var promotion = new Promotion
            {
                PartnerId = partnerId,
                PromotionCode = promotionCode,
                PromotionName = "Test Promotion",
                StartTime = pastStartTime,
                EndTime = futureEndTime
            };

            promotionDataAccess.GetPromotion(partnerId, promotionCode).Returns(promotion);

            // Act
            var result = await partnerDataService.GetPromotion(partnerId, promotionCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(promotionCode, result.PromotionCode);
            Assert.Equal("Test Promotion", result.PromotionName);
            Assert.Equal(PromotionStatus.Running, result.Status);
            Assert.Equal(pastStartTime, result.StartTime);
            Assert.Equal(futureEndTime, result.EndTime);
            await promotionDataAccess.Received(1).GetPromotion(partnerId, promotionCode);
        }

        [Fact]
        public async Task GetPromotion_WithValidData_ShouldReturnPromotionWithCompletedStatus()
        {
            // Arrange
            var partnerId = "PARTNER123";
            var promotionCode = "PROMO123";
            var pastStartTime = DateTime.UtcNow.AddDays(-10);
            var pastEndTime = DateTime.UtcNow.AddDays(-1);
            
            var promotion = new Promotion
            {
                PartnerId = partnerId,
                PromotionCode = promotionCode,
                PromotionName = "Test Promotion",
                StartTime = pastStartTime,
                EndTime = pastEndTime
            };

            promotionDataAccess.GetPromotion(partnerId, promotionCode).Returns(promotion);

            // Act
            var result = await partnerDataService.GetPromotion(partnerId, promotionCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(promotionCode, result.PromotionCode);
            Assert.Equal("Test Promotion", result.PromotionName);
            Assert.Equal(PromotionStatus.Completed, result.Status);
            Assert.Equal(pastStartTime, result.StartTime);
            Assert.Equal(pastEndTime, result.EndTime);
            await promotionDataAccess.Received(1).GetPromotion(partnerId, promotionCode);
        }

        [Theory]
        [InlineData(null, "PROMO123")]
        [InlineData("", "PROMO123")]
        [InlineData(" ", "PROMO123")]
        [InlineData("PARTNER123", null)]
        [InlineData("PARTNER123", "")]
        [InlineData("PARTNER123", " ")]
        public async Task GetPromotion_WithInvalidData_ShouldReturnNull(string partnerId, string promotionCode)
        {
            // Act
            var result = await partnerDataService.GetPromotion(partnerId, promotionCode);

            // Assert
            Assert.Null(result);
            await promotionDataAccess.DidNotReceive().GetPromotion(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task GetPromotion_WithNonExistentPromotion_ShouldReturnNull()
        {
            // Arrange
            var partnerId = "PARTNER123";
            var promotionCode = "PROMO123";
            
            promotionDataAccess.GetPromotion(partnerId, promotionCode).Returns((Promotion)null);
            
            // Act
            var result = await partnerDataService.GetPromotion(partnerId, promotionCode);
            
            // Assert
            Assert.Null(result);
            await promotionDataAccess.Received(1).GetPromotion(partnerId, promotionCode);
        }
    }
}