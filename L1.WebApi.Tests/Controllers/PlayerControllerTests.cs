using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Award;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.Promotion;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.WebApi.Controllers;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sentry;
using Xunit;
using System.Linq;

namespace L1.WebApi.Tests.Controllers
{
    public class PlayerControllerTests
    {
        private readonly IPartnerCustomerFactory partnerCustomerFactory;
        private readonly IPartnerCustomerServices oWCustomerService;
        private readonly IPartnerPlayerIdConverter partnerPlayerIdConverter;
        private readonly ISentryClient sentryClient;

        private readonly PlayerController controller;
        private readonly Fixture fixture = new();

        public PlayerControllerTests()
        {
            partnerCustomerFactory = Substitute.For<IPartnerCustomerFactory>();
            oWCustomerService = Substitute.For<IPartnerCustomerServices>();
            partnerPlayerIdConverter = Substitute.For<IPartnerPlayerIdConverter>();
            sentryClient = Substitute.For<ISentryClient>();
            partnerPlayerIdConverter.ConvertPlayerId(Arg.Any<string>(), Arg.Any<string>()).Returns(x => x.ArgAt<string>(1));

            controller = new PlayerController(partnerCustomerFactory, partnerPlayerIdConverter, sentryClient);
        }

        [Fact]
        public async Task EnterPortal_Always_ReturnOkResult()
        {
            // Arrange
            var result = fixture.Create<PartnerResponse<PartnerEnterPortalResult>>();
            var model = fixture.Create<MemberEnterPortal>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.EnterPortal(Arg.Any<PartnerEnterPortalCall>()).Returns(result);

            // Act
            var actualValue = await controller.EnterPortal(model);

            // Assert
            var objectResult = actualValue.Result as OkObjectResult;
            Assert.NotNull(objectResult);

            var responseValue = objectResult?.Value as PartnerResponse<PartnerEnterPortalResult>;
            Assert.NotNull(responseValue);

            Assert.Equal(result, responseValue);
        }

        [Fact]
        public async Task EnterPortal_NotSupportedPartner_ReturnBadRequest()
        {
            // Arrange
            var model = fixture.Create<MemberEnterPortal>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.EnterPortal(model);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actualValue.Result);
        }

        [Fact]
        public async Task Balance_Always_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<GetBalance>();
            var result = fixture.Create<PartnerResponse<PartnerGetBalanceResult>>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.GetBalance(Arg.Any<PartnerGetBalanceCall>()).Returns(result);

            // Act
            var actualValue = await controller.Balance(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async Task Balance_NotSupportedPartner_ReturnBadRequest()
        {
            // Arrange
            var model = fixture.Create<GetBalance>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.Balance(model);

            // Assert
            Assert.IsType<BadRequestResult>(actualValue);
        }

        [Fact]
        public async Task PlaceBet_Always_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<PlaceBet>();
            var result = fixture.Create<PartnerResponse<PartnerPlaceBetResult>>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.PlaceBet(Arg.Any<PartnerPlaceBetCall>(), Arg.Any<byte>(), Arg.Any<Dictionary<string, string>>()).Returns(result);

            // Act
            var actualValue = await controller.PlaceBet(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async Task PlaceBet_NotSupportedPartner_ReturnBadRequest()
        {
            // Arrange
            var model = fixture.Create<PlaceBet>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.PlaceBet(model);

            // Assert
            Assert.IsType<BadRequestResult>(actualValue);
        }

        [Fact]
        public async Task EndGame_Always_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<EndGame>();
            var result = fixture.Create<PartnerResponse<PartnerEndGameResult>>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.EndGame(Arg.Any<PartnerEndGameCall>()).Returns(result);

            // Act
            var actualValue = await controller.EndGame(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async Task EndGame_NotSupportedPartner_ReturnBadRequest()
        {
            // Arrange
            var model = fixture.Create<EndGame>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.EndGame(model);

            // Assert
            Assert.IsType<BadRequestResult>(actualValue);
        }

        [Fact]
        public async Task VoidGame_Always_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<VoidGame>();
            var result = fixture.Create<PartnerResponse<PartnerVoidGameResult>>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.VoidGame(Arg.Any<PartnerVoidGameCall>()).Returns(result);

            // Act
            var actualValue = await controller.VoidGame(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async Task VoidGame_NotSupportedPartner_ReturnBadRequest()
        {
            // Arrange
            var model = fixture.Create<VoidGame>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.VoidGame(model);

            // Assert
            Assert.IsType<BadRequestResult>(actualValue);
        }

        [Fact]
        public async Task GetBetLimit_Always_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<GetBetLimit>();
            var result = fixture.Create<PartnerResponse<PartnerGetBetLimitResult>>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.GetBetLimit(Arg.Any<PartnerGetBetLimitCall>()).Returns(result);

            // Act
            var actualValue = await controller.GetBetLimit(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async Task GetBetLimit_NotSupportedPartner_ReturnBadRequest()
        {
            // Arrange
            var model = fixture.Create<GetBetLimit>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.GetBetLimit(model);

            // Assert
            Assert.IsType<BadRequestResult>(actualValue);
        }

        [Fact]
        public async Task Award_Always_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<Award>();
            var result = fixture.Create<PartnerResponse<PartnerAwardResult>>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.Award(Arg.Any<PartnerAwardCall>()).Returns(result);

            // Act
            var actualValue = await controller.Award(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async Task Award_NotSupportedPartner_ReturnBadRequest()
        {
            // Arrange
            var model = fixture.Create<Award>();
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.Award(model);

            // Assert
            Assert.IsType<BadRequestResult>(actualValue);
        }

        [Fact]
        public async Task StartPromotion_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var model = new StartPromotion
            {
                OperatorId = "OP123",
                PlayerId = "PLAYER456",
                SiteId = "SITE789",
                PromotionCode = "PROMO2023",
                Start = "2023/12/01 00:00:00.000 +00:00",
                End = "2023/12/31 23:59:59.999 +00:00"
            };
            
            var result = new PartnerResponse<PartnerStartPromotionResult>
            {
                Result = new PartnerStartPromotionResult
                {
                    PromotionCode = "PROMO2023",
                    ErrorCode = 0,
                    ErrorDescription = ""
                },
                TimeStamp = DateTimeOffset.UtcNow
            };
            
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.StartPromotion(Arg.Any<PartnerStartPromotionCall>()).Returns(result);

            // Act
            var actualValue = await controller.StartPromotion(model);

            // Assert
            var objectResult = actualValue as OkObjectResult;
            Assert.NotNull(objectResult);
            
            var responseValue = objectResult?.Value as PartnerResponse<PartnerStartPromotionResult>;
            Assert.NotNull(responseValue);
            Assert.Equal(result.Result.PromotionCode, responseValue.Result.PromotionCode);
            Assert.Equal(0, responseValue.Result.ErrorCode);
        }

        [Fact]
        public async Task StartPromotion_InvalidOperatorId_ReturnsBadRequestResult()
        {
            // Arrange
            var model = new StartPromotion
            {
                OperatorId = "INVALID",
                PlayerId = "PLAYER456",
                SiteId = "SITE789",
                PromotionCode = "PROMO2023",
                Start = "2023/12/01 00:00:00.000 +00:00",
                End = "2023/12/31 23:59:59.999 +00:00"
            };
            
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(default(IPartnerCustomerServices));

            // Act
            var actualValue = await controller.StartPromotion(model);

            // Assert
            var badRequestResult = actualValue as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal("Invalid operator ID", badRequestResult.Value);
        }

        [Fact]
        public async Task StartPromotion_MissingPromotionCode_ReturnsBadRequestWithErrorCode1007()
        {
            // Arrange
            var model = new StartPromotion
            {
                OperatorId = "OP123",
                PlayerId = "PLAYER456",
                SiteId = "SITE789",
                PromotionCode = "", // Empty promotion code
                Start = "2023/12/01 00:00:00.000 +00:00",
                End = "2023/12/31 23:59:59.999 +00:00"
            };
            
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);

            // Act
            var actualValue = await controller.StartPromotion(model);

            // Assert
            var badRequestResult = actualValue as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            
            var errorResponse = badRequestResult.Value as object;
            Assert.NotNull(errorResponse);
            
            // Convert to dictionary to access dynamic properties
            var properties = errorResponse.GetType().GetProperties();
            var errorCode = properties.FirstOrDefault(p => p.Name == "ErrorCode")?.GetValue(errorResponse);
            var errorDescription = properties.FirstOrDefault(p => p.Name == "ErrorDescription")?.GetValue(errorResponse);
            
            Assert.Equal(1007, errorCode);
            Assert.Equal("PromotionCode is required", errorDescription);
        }

        [Fact]
        public async Task StartPromotion_MissingStartDate_ReturnsBadRequestWithErrorCode1007()
        {
            // Arrange
            var model = new StartPromotion
            {
                OperatorId = "OP123",
                PlayerId = "PLAYER456",
                SiteId = "SITE789",
                PromotionCode = "PROMO2023",
                Start = "", // Empty start date
                End = "2023/12/31 23:59:59.999 +00:00"
            };
            
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);

            // Act
            var actualValue = await controller.StartPromotion(model);

            // Assert
            var badRequestResult = actualValue as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            
            var errorResponse = badRequestResult.Value as object;
            Assert.NotNull(errorResponse);
            
            // Convert to dictionary to access dynamic properties
            var properties = errorResponse.GetType().GetProperties();
            var errorCode = properties.FirstOrDefault(p => p.Name == "ErrorCode")?.GetValue(errorResponse);
            var errorDescription = properties.FirstOrDefault(p => p.Name == "ErrorDescription")?.GetValue(errorResponse);
            
            Assert.Equal(1007, errorCode);
            Assert.Equal("Start and End dates are required", errorDescription);
        }

        [Fact]
        public async Task StartPromotion_InvalidDateFormat_ReturnsBadRequestWithErrorCode1007()
        {
            // Arrange
            var model = new StartPromotion
            {
                OperatorId = "OP123",
                PlayerId = "PLAYER456",
                SiteId = "SITE789",
                PromotionCode = "PROMO2023",
                Start = "Invalid-Date-Format", // Invalid date format
                End = "2023/12/31 23:59:59.999 +00:00"
            };
            
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);

            // Act
            var actualValue = await controller.StartPromotion(model);

            // Assert
            var badRequestResult = actualValue as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            
            var errorResponse = badRequestResult.Value as object;
            Assert.NotNull(errorResponse);
            
            // Convert to dictionary to access dynamic properties
            var properties = errorResponse.GetType().GetProperties();
            var errorCode = properties.FirstOrDefault(p => p.Name == "ErrorCode")?.GetValue(errorResponse);
            var errorDescription = properties.FirstOrDefault(p => p.Name == "ErrorDescription")?.GetValue(errorResponse);
            
            Assert.Equal(1007, errorCode);
            Assert.Equal("Invalid date format. Please use format: yyyy/MM/dd HH:mm:ss.fff +00:00", errorDescription);
        }

        [Fact]
        public async Task StartPromotion_ServiceThrowsException_ReturnsBadRequestWithErrorCode1007()
        {
            // Arrange
            var model = new StartPromotion
            {
                OperatorId = "OP123",
                PlayerId = "PLAYER456",
                SiteId = "SITE789",
                PromotionCode = "PROMO2023",
                Start = "2023/12/01 00:00:00.000 +00:00",
                End = "2023/12/31 23:59:59.999 +00:00"
            };
            
            partnerCustomerFactory.GetCustomerService(Arg.Any<string>()).Returns(oWCustomerService);
            oWCustomerService.StartPromotion(Arg.Any<PartnerStartPromotionCall>())
                .Returns(Task.FromException<PartnerResponse<PartnerStartPromotionResult>>(new Exception("Test exception")));

            // Act
            var actualValue = await controller.StartPromotion(model);

            // Assert
            var badRequestResult = actualValue as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            
            var errorResponse = badRequestResult.Value as object;
            Assert.NotNull(errorResponse);
            
            // Convert to dictionary to access dynamic properties
            var properties = errorResponse.GetType().GetProperties();
            var errorCode = properties.FirstOrDefault(p => p.Name == "ErrorCode")?.GetValue(errorResponse);
            var errorDescription = properties.FirstOrDefault(p => p.Name == "ErrorDescription")?.GetValue(errorResponse);
            
            Assert.Equal(1007, errorCode);
            Assert.Equal("An error occurred while processing the request", errorDescription);
        }
    }
}