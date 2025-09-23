using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using L1.Shared.Configurations;
using L1.Shared.Constants;
using Polly;
using Sentry;

namespace L1.Features.SabaPlayCommunicators
{
    public class SabaPlayCustomerService : IPartnerCustomerServices
    {
        private readonly ISabaPlayService sabaPlayService;
        private readonly ILogService logService;
        private readonly SabaPlayEndPoints endpoints;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;
        private readonly INatsService natsService;

        public SabaPlayCustomerService(
            ISabaPlayService SabaPlayService,
            ILogService logService,
            INatsService natsService,
            SabaPlayEndPoints endpoints,
            ISentryClient sentryClient,
            IAppSettings appSettings)
        {
            this.sabaPlayService = SabaPlayService;
            this.logService = logService;
            this.natsService = natsService;
            this.endpoints = endpoints;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public string OperatorId { get; set; }

        public async Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        {
            var response = await sabaPlayService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);
            if (response.Result.IsSuccessful() && appSettings.LogEnabling.EnterPortal)
            {
                await logService.LogEnterPortalAsync(OperatorId, response);
            }

            return response;
        }

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => sabaPlayService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);

        public async Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        {
            var response = await sabaPlayService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);

            if (response.Result.IsSuccessful() && appSettings.LogEnabling.GetBetLimit)
            {
                await logService.LogGetBetLimitAsync(OperatorId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub, Dictionary<string, string> metadata = null)
        {
            var response = await sabaPlayService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(appSettings.PartnerIds.SabaPlay, response, system);
                await natsService.PublishAsync(StreamSubjects.BetCreated, new
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
                    PlayerId = SabaPlayFormatter.FormatPlayerId(call.SiteId, call.PlayerId),
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
            var response = await sabaPlayService.Post<PartnerEndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(appSettings.PartnerIds.SabaPlay, response);
                await natsService.PublishAsync(StreamSubjects.BetSettled, new
                {
                    OperatorId,
                    call.Seq,
                    PlayerId = SabaPlayFormatter.FormatPlayerId(call.SiteId, call.PlayerId),
                    call.SiteId,
                    call.TimeStamp,
                    call.GameRoundId,
                    call.GameTypeId,
                    call.TotalAmount,
                    call.TotalWin,
                    call.ValidBetAmount,
                    call.EndTime,
                    call.FreeBet,
                    call.PromotionId,
                    call.SiteName,
                    call.BaseTotalAmount
                });
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await sabaPlayService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(appSettings.PartnerIds.SabaPlay, response);
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
                .ExecuteAsync(() => sabaPlayService.Post<HitJackpotResult>(endpoints.HitJackpot, call));

            if (response.Result.IsSuccessful())
            {
                await logService.LogHitJackpotAsync(appSettings.PartnerIds.SabaPlay, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call HitJackpot failed for Partner {appSettings.PartnerIds.SabaPlay} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerAwardResult>> Award(PartnerAwardCall call)
        {
            var response = await sabaPlayService.Post<PartnerAwardResult>(endpoints.Award, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogAwardAsync(OperatorId, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call Award failed for Partner {appSettings.PartnerIds.SabaPlay} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }
    }
}