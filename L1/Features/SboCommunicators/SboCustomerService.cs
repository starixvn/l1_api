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

namespace L1.Features.SboCommunicators
{
    public class SboCustomerService : IPartnerCustomerServices
    {
        private readonly ISboService sboService;
        private readonly ILogService logService;
        private readonly SboEndPoints endpoints;
        private readonly ISentryClient sentryClient;
        private readonly IAppSettings appSettings;

        public SboCustomerService(
            ISboService SboService,
            ILogService logService,
            SboEndPoints endpoints,
            ISentryClient sentryClient,
            IAppSettings appSettings)
        {
            this.sboService = SboService;
            this.logService = logService;
            this.endpoints = endpoints;
            this.sentryClient = sentryClient;
            this.appSettings = appSettings;
        }
        public string OperatorId { get; set; }

        public Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        => sboService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call);

        public Task<PartnerResponse<PartnerGetBalanceResult>> GetBalance(PartnerGetBalanceCall call)
        => sboService.Post<PartnerGetBalanceResult>(endpoints.GetBalance, call);

        public Task<PartnerResponse<PartnerGetBetLimitResult>> GetBetLimit(PartnerGetBetLimitCall call)
        => sboService.Post<PartnerGetBetLimitResult>(endpoints.GetBetLimit, call);

        public async Task<PartnerResponse<PartnerPlaceBetResult>> PlaceBet(PartnerPlaceBetCall call, byte system = AppTypes.SabaClub, Dictionary<string, string> metadata = null)
        {
            var response = await sboService.Post<PartnerPlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(appSettings.PartnerIds.SBO, response, system);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await sboService.Post<PartnerEndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(appSettings.PartnerIds.SBO, response);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerVoidGameResult>> VoidGame(PartnerVoidGameCall call)
        {
            var response = await sboService.Post<PartnerVoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(appSettings.PartnerIds.SBO, response);
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
                .ExecuteAsync(() => sboService.Post<HitJackpotResult>(endpoints.HitJackpot, call));

            if (response.Result.IsSuccessful())
            {
                await logService.LogHitJackpotAsync(appSettings.PartnerIds.SBO, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call HitJackpot failed for Partner {appSettings.PartnerIds.SBO} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerAwardResult>> Award(PartnerAwardCall call)
        {
            var response = await sboService.Post<PartnerAwardResult>(endpoints.Award, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogAwardAsync(OperatorId, response);
            }
            else
            {
                sentryClient.CaptureMessage($"Call Award failed for Partner {appSettings.PartnerIds.SBO} - CustId {call.PlayerId}", SentryLevel.Fatal);
            }

            return response;
        }
    }
}