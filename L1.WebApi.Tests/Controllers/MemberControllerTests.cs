using AutoFixture;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.DailySummary;
using L1.Features.AdminApiCommunicators.RetrieveEndGameInfo;
using L1.Features.OWCommunicators;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.GetBetLimit;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
using L1.Features.Sites;
using L1.WebApi.Controllers.V1;
using L1.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace L1.WebApi.Tests.Controllers
{
    public class MemberControllerTests
    {
        private readonly IOWCustomerService oWCustomerService;
        private readonly IAdminApiService adminApiService;
        private readonly ISiteDataService siteDataService;
        private readonly ILogService logService;

        private readonly MemberController controller;
        private readonly Fixture fixture = new Fixture();

        public MemberControllerTests()
        {
            oWCustomerService = Substitute.For<IOWCustomerService>();
            adminApiService = Substitute.For<IAdminApiService>();
            siteDataService = Substitute.For<ISiteDataService>();
            logService = Substitute.For<ILogService>();

            controller = new MemberController(oWCustomerService, adminApiService, siteDataService, logService);
        }

        [Fact]
        public async void EnterPortal_Always_ReturnOkResult()
        {
            // Arrange 
            var model = fixture.Create<MemberEnterPortal>();
            var result = fixture.Create<OWResponse<EnterPortalResult>>();
            oWCustomerService.EnterPortal(Arg.Any<EnterPortalCall>()).Returns(result);

            // Act
            var actualValue = await controller.EnterPortal(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async void Balance_Always_ReturnOkResult()
        {
            // Arrange 
            var model = fixture.Create<GetBalance>();
            var result = fixture.Create<OWResponse<GetBalanceResult>>();
            oWCustomerService.GetBalance(Arg.Any<GetBalanceCall>()).Returns(result);

            // Act
            var actualValue = await controller.Balance(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async void PlaceBet_Always_ReturnOkResult()
        {
            // Arrange 
            var model = fixture.Create<PlaceBet>();
            var result = fixture.Create<OWResponse<PlaceBetResult>>();
            oWCustomerService.PlaceBet(Arg.Any<PlaceBetCall>(), Arg.Any<byte>()).Returns(result);

            // Act
            var actualValue = await controller.PlaceBet(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async void EndGame_Always_ReturnOkResult()
        {
            // Arrange 
            var model = fixture.Create<EndGame>();
            var result = fixture.Create<OWResponse<EndGameResult>>();
            oWCustomerService.EndGame(Arg.Any<EndGameCall>()).Returns(result);

            // Act
            var actualValue = await controller.EndGame(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async void VoidGame_Always_ReturnOkResult()
        {
            // Arrange 
            var model = fixture.Create<VoidGame>();
            var result = fixture.Create<OWResponse<VoidGameResult>>();
            oWCustomerService.VoidGame(Arg.Any<VoidGameCall>()).Returns(result);

            // Act
            var actualValue = await controller.VoidGame(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async void RetrieveEndGameInfo_ValidClient_ReturnOkResult()
        {
            // Arrange 
            var model = fixture.Create<EndGameInfoRequest>();
            siteDataService.VerifyClient(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var result = fixture.Create<AdminApiResponse<RetrieveEndGameInfoResult>>();
            adminApiService.RetrieveEndGameInfoAsync(Arg.Any<RetrieveEndGameInfoRequest>()).Returns(result);

            // Act
            var actualValue = await controller.RetrieveEndGameInfo(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async void RetrieveEndGameInfo_InvalidClient_ReturnUnauthorizedResult()
        {
            // Arrange 
            var model = fixture.Create<EndGameInfoRequest>();
            model.Auth = null;
            siteDataService.VerifyClient(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            var result = fixture.Create<AdminApiResponse<RetrieveEndGameInfoResult>>();
            adminApiService.RetrieveEndGameInfoAsync(Arg.Any<RetrieveEndGameInfoRequest>()).Returns(result);

            // Act
            var actualValue = await controller.RetrieveEndGameInfo(model) as UnauthorizedResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async void DailySummarize_InvalidClient_ReturnUnauthorizedResult()
        {
            // Arrange 
            var model = fixture.Create<DailySummary>();
            model.Auth = null;
            siteDataService.VerifyClient(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            var result = fixture.Create<AdminApiResponse<DailySummaryResult>>();
            adminApiService.DailySummary(Arg.Any<DailySummaryRequest>()).Returns(result);

            // Act
            var actualValue = await controller.DailySummarize(model) as UnauthorizedResult;

            // Assert
            Assert.NotNull(actualValue);
        }

        [Fact]
        public async void DailySummarize_ValidClient_ReturnUnauthorizedResult()
        {
            // Arrange 
            var model = fixture.Create<DailySummary>();
            siteDataService.VerifyClient(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            var result = fixture.Create<AdminApiResponse<DailySummaryResult>>();
            adminApiService.DailySummary(Arg.Any<DailySummaryRequest>()).Returns(result);

            // Act
            var actualValue = await controller.DailySummarize(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }

        [Fact]
        public async void GetBetLimit_Always_ReturnOkResult()
        {
            // Arrange 
            var model = fixture.Create<GetBetLimit>();
            var result = fixture.Create<OWResponse<GetBetLimitResult>>();
            oWCustomerService.GetBetLimit(Arg.Any<GetBetLimitCall>()).Returns(result);

            // Act
            var actualValue = await controller.GetBetLimit(model) as OkObjectResult;

            // Assert
            Assert.Equal(result, actualValue.Value);
        }
    }
}
