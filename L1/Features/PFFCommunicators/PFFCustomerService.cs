using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Features.OWCommunicators;
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
using Sentry.Protocol;

namespace L1.Features.PFFCommunicators
{
    public class PFFCustomerService : IPartnerCustomerServices
    {
        private readonly IPFFService PFFService;
        private readonly ILogService logService;
        private readonly PFFEndPoints endpoints;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public PFFCustomerService(
            IPFFService PFFService,
            ILogService logService,
            PFFEndPoints endpoints,
            ISentryClient sentryClient,
            IAppSettings appSettings)
        {
            this.PFFService = PFFService;
            this.logService = logService;
            this.endpoints = endpoints;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }

        public string OperatorId { get; set; }

        public Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        => PFFService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => PFFService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);

        public Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        => PFFService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub, Dictionary<string, string> metadata = null)
        {
            var response = await PFFService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(appSettings.PartnerIds.PFF, response, system);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await PFFService.Post<PartnerEndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(appSettings.PartnerIds.PFF, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await PFFService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(appSettings.PartnerIds.PFF, response);
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
                .ExecuteAsync(() => PFFService.Post<HitJackpotResult>(endpoints.HitJackpot, call));

            if (response.Result.IsSuccessful())
            {
                await logService.LogHitJackpotAsync(appSettings.PartnerIds.PFF, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call HitJackpot failed for Partner {appSettings.PartnerIds.PFF} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerAwardResult>> Award(PartnerAwardCall call)
        {
            var response = await PFFService.Post<PartnerAwardResult>(endpoints.Award, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogAwardAsync(OperatorId, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call Award failed for Partner {appSettings.PartnerIds.PFF} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }
    }
}