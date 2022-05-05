using System.Threading.Tasks;
using AutoFixture;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Features.Partners;
using L1.Features.SGApiCommunicators;
using L1.WebApi.Controllers;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace L1.WebApi.Tests.Controllers
{
    public class PlayerControllerTests
    {
        private readonly IPartnerCustomerFactory partnerCustomerFactory;
        private readonly IPartnerCustomerServices oWCustomerService;
        private readonly IPartnerDataService partnerDataService;
        private readonly IPartnerRequestAuthentication partnerRequestAuthentication;
        private readonly ISGApiService sgApiService;

        private readonly PlayerController controller;
        private readonly Fixture fixture = new();

        public PlayerControllerTests()
        {
            partnerCustomerFactory = Substitute.For<IPartnerCustomerFactory>();
            oWCustomerService = Substitute.For<IPartnerCustomerServices>();
            partnerDataService = Substitute.For<IPartnerDataService>();
            partnerRequestAuthentication = Substitute.For<IPartnerRequestAuthentication>();
            sgApiService = Substitute.For<ISGApiService>();
            controller = new PlayerController(partnerCustomerFactory, sgApiService, partnerDataService, partnerRequestAuthentication);
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
            var actualValue = await controller.EnterPortal(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
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
            Assert.IsType<BadRequestResult>(actualValue);
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
            oWCustomerService.PlaceBet(Arg.Any<PartnerPlaceBetCall>(), Arg.Any<byte>()).Returns(result);

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
        public async Task RetrieveEndGameInfo_ValidClient_ReturnOkResult()
        {
            // Arrange
            var model = fixture.Create<RetrieveEndRoundInfo>();
            partnerRequestAuthentication.Verify(Arg.Any<string>(), Arg.Any<HttpRequest>()).Returns(true);

            var result = fixture.Create<SGResponse<RetrieveEndRoundInfoResult>>();
            sgApiService.RetrieveEndRoundInfoAsync(Arg.Any<RetrieveEndRoundInfoRequest>()).Returns(result);

            // Act
            var actualValue = await controller.RetrieveEndRoundInfo(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async Task RetrieveEndGameInfo_InvalidClient_ReturnUnauthorizedResult()
        {
            // Arrange
            var model = fixture.Create<RetrieveEndRoundInfo>();
            model.Auth = null;
            partnerRequestAuthentication.Verify(Arg.Any<string>(), Arg.Any<HttpRequest>()).Returns(false);

            // Act
            var actualValue = await controller.RetrieveEndRoundInfo(model);

            // Assert
            Assert.IsType<UnauthorizedResult>(actualValue);
        }

        [Fact]
        public async Task DailySummarize_InvalidClient_ReturnUnauthorizedResult()
        {
            // Arrange
            var model = fixture.Create<DailySummary>();
            model.Auth = null;
            partnerRequestAuthentication.Verify(Arg.Any<string>(), Arg.Any<HttpRequest>()).Returns(false);

            // Act
            var actualValue = await controller.DailySummarize(model);

            // Assert
            Assert.IsType<UnauthorizedResult>(actualValue);
        }

        [Fact]
        public async Task DailySummarize_ValidClient_ReturnResult()
        {
            // Arrange
            var model = fixture.Create<DailySummary>();
            partnerRequestAuthentication.Verify(Arg.Any<string>(), Arg.Any<HttpRequest>()).Returns(true);

            var result = fixture.Create<SGResponse<DailySummaryResult>>();
            sgApiService.DailySummary(Arg.Any<DailySummaryRequest>()).Returns(result);

            // Act
            var actualValue = await controller.DailySummarize(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }
    }
}