using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Award;
using L1.Features.PartnerCommunicators.EndGame;
using L1.Features.PartnerCommunicators.EnterPortal;
using L1.Features.PartnerCommunicators.GetBalance;
using L1.Features.PartnerCommunicators.GetBetLimit;
using L1.Features.PartnerCommunicators.HitJackpot;
using L1.Features.PartnerCommunicators.Log;
using L1.Features.PartnerCommunicators.PlaceBet;
using L1.Features.PartnerCommunicators.VoidGame;
using L1.Features.SGCommunicators.Award;
using L1.Features.SGCommunicators.EndGame;
using L1.Features.SGCommunicators.EnterPortal;
using L1.Features.SGCommunicators.GetBalance;
using L1.Features.SGCommunicators.GetBetLimit;
using L1.Features.SGCommunicators.PlaceBet;
using L1.Features.SGCommunicators.VoidGame;
using L1.Shared.Configurations;
using L1.Shared.Constants;
using Polly;
using Sentry;

namespace L1.Features.SGCommunicators
{
    public class SGCustomerService : IPartnerCustomerServices
    {
        private readonly ILogService logService;
        private readonly IAppSettings appSettings;
        private readonly IAdminApiService adminApiService;
        private readonly ISentryClient sentryClient;
        private readonly ISGService sgService;
        private readonly INatsService natsService;
        private readonly SGEndPoints endpoints;

        public SGCustomerService(
            ISGService sgService,
            INatsService natsService,
            ILogService logService,
            SGEndPoints endpoints,
            ISentryClient sentryClient,
            IAppSettings appSettings,
            IAdminApiService adminApiService)
        {
            this.sgService = sgService;
            this.natsService = natsService;
            this.logService = logService;
            this.endpoints = endpoints;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
            this.adminApiService = adminApiService;
        }

        public string OperatorId { get; set; }

        public async Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        {
            var response = await sgService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, new SGEnterPortalCall((int)call.PlayerId, call.SiteId, call.OperatorToken), OperatorId);

            if (response.Result.IsSuccessful() && appSettings.LogEnabling.EnterPortal)
            {
                await logService.LogEnterPortalAsync(OperatorId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        {
            return await sgService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, new SGGetBalanceCall((int)call.PlayerId, call.SiteId, call.OperatorToken), OperatorId);
        }

        public async Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        {
            var response = await sgService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, new SGGetBetLimitCall((int)call.PlayerId, call.SiteId, call.OperatorToken), OperatorId);

            if (response.Result.IsSuccessful() && appSettings.LogEnabling.GetBetLimit)
            {
                await logService.LogGetBetLimitAsync(OperatorId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub, Dictionary<string, string> metadata = null)
        {
            var response = await sgService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, new SGPlaceBetCall(
                (int)call.PlayerId,
                call.SiteId,
                call.GameRoundId,
                call.GameTypeId,
                call.ChoiceId,
                call.Amount,
                call.Currency,
                call.RoundEndTime,
                call.Ip,
                call.FreeSpin,
                call.ContributionAmount,
                call.ContributionRate,
                call.FreeBet,
                call.OperatorToken,
                call.OperatorId,
                call.UserId), OperatorId);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(OperatorId, response, system, metadata);
                await natsService.PublishAsync("bet.created", new
                {
                    call.GameRoundId,
                    call.GameTypeId,
                    call.ChoiceId,
                    call.Amount,
                    call.Currency,
                    call.RoundEndTime,
                    call.Ip,
                    call.FreeSpin,
                    call.ContributionAmount,
                    call.ContributionRate,
                    call.OperatorToken,
                    call.FreeBet,
                    call.OperatorId,
                    call.UserId,
                    call.Seq,
                    call.PlayerId,
                    call.SiteId,
                    call.TimeStamp,
                    System = system,
                    Metadata = metadata
                });
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await sgService.Post<PartnerEndGameResult>(endpoints.EndGame, new SGEndGameCall(
                (int)call.PlayerId,
                call.SiteId,
                call.GameRoundId,
                call.GameTypeId,
                call.TotalAmount,
                call.TotalWin,
                call.ValidBetAmount,
                call.EndTime ?? DateTimeOffset.UtcNow,
                call.FreeBet), OperatorId);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(OperatorId, response);
                await natsService.PublishAsync("bet.settled", call);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await sgService.Post<PartnerVoidGameResult>(endpoints.VoidGame, new SGVoidGameCall(
                (int)call.PlayerId,
                call.SiteId,
                call.GameRoundId,
                call.GameTypeId,
                call.Reason,
                call.FreeBet), OperatorId);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(OperatorId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<HitJackpotResult>> HitJackpot(HitJackpotCall call)
        {
            const double retryDelayInSeconds = 30;
            const int numberOfRetries = 6;

            var response = await Policy
                .HandleResult<PartnerResponse<HitJackpotResult>>(response => response.Result.IsFailed())
                .WaitAndRetryAsync(numberOfRetries, retryAttempt => TimeSpan.FromSeconds(retryDelayInSeconds))
                .ExecuteAsync(() => sgService.Post<HitJackpotResult>(endpoints.HitJackpot, call, OperatorId));

            if (response.Result.IsSuccessful())
            {
                await logService.LogHitJackpotAsync(OperatorId, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call HitJackpot failed for Partner {OperatorId} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerAwardResult>> Award(PartnerAwardCall call)
        {
            var response = await sgService.Post<PartnerAwardResult>(endpoints.Award, new SGAwardCall((int)call.PlayerId, call.SiteId, call.GameTypeId, call.Amount, call.Currency), OperatorId);

            if (response.Result.IsSuccessful())
            {
                await logService.LogAwardAsync(OperatorId, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call Award failed for Partner {OperatorId} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }
    }
}