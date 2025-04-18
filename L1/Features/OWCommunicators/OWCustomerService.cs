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
using L1.Shared.Configurations;
using L1.Shared.Constants;
using Polly;
using Sentry;

namespace L1.Features.OWCommunicators
{
    public class OWCustomerService : IPartnerCustomerServices
    {
        private readonly IOWService oWService;
        private readonly ILogService logService;
        private readonly OWEndPoints endpoints;
        private readonly IAppSettings appSettings;
        private readonly ISentryClient sentryClient;
        private readonly INatsService natsService;
        private readonly IPartnerEndGameHandler partnerEndGameHandler;

        public OWCustomerService(
            IOWService oWService,
            ILogService logService,
            IAppSettings appSettings,
            OWEndPoints endpoints,
            ISentryClient sentryClient,
            INatsService natsService,
            IPartnerEndGameHandler partnerEndGameHandler)
        {
            this.oWService = oWService;
            this.logService = logService;
            this.endpoints = endpoints;
            this.appSettings = appSettings;
            this.sentryClient = sentryClient;
            this.natsService = natsService;
            this.partnerEndGameHandler = partnerEndGameHandler;
        }

        public string OperatorId { get; set; }

        public async Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        {
            var response = await oWService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call, OperatorId);

            if (response.Result.IsSuccessful() && appSettings.LogEnabling.EnterPortal)
            {
                await logService.LogEnterPortalAsync(OperatorId, response);
            }

            return response;
        }

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => oWService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call, OperatorId);

        public async Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        {
            var response = await oWService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call, OperatorId);

            if (response.Result.IsSuccessful() && appSettings.LogEnabling.GetBetLimit)
            {
                await logService.LogGetBetLimitAsync(OperatorId, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub, Dictionary<string, string> metadata = null)
        {
            var response = await oWService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call, OperatorId);

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
            var response = await oWService.Post<PartnerEndGameResult>(endpoints.EndGame, call, OperatorId);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(OperatorId, response);
                await natsService.PublishAsync("bet.settled", call);
            }
            else 
            { 
                await partnerEndGameHandler.HandleFailedEndGame(call, response, OperatorId);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await oWService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call, OperatorId);

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
                .ExecuteAsync(() => oWService.Post<HitJackpotResult>(endpoints.HitJackpot, call, OperatorId));

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
            var response = await oWService.Post<PartnerAwardResult>(endpoints.Award, call, OperatorId);

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