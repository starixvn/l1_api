using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.AdminApiCommunicators.NotifyHighStake;
using L1.Features.PartnerCommunicators;
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

namespace L1.Features.OWCommunicators
{
    public class OWCustomerService : IPartnerCustomerServices
    {
        private readonly IOWService oWService;
        private readonly ILogService logService;
        private readonly OWEndPoints endpoints;
        private readonly IAppSettings appSettings;
        private readonly IAdminApiService adminApiSerivce;
        private readonly ISentryClient sentryClient;

        public OWCustomerService(
            IOWService oWService,
            ILogService logService,
            IAppSettings appSettings,
            IAdminApiService adminApiSerivce,
            OWEndPoints endpoints,
            ISentryClient sentryClient)
        {
            this.oWService = oWService;
            this.logService = logService;
            this.endpoints = endpoints;
            this.appSettings = appSettings;
            this.adminApiSerivce = adminApiSerivce;
            this.sentryClient = sentryClient;
        }

        public string OperatorId { get; set; }

        public async Task<PartnerResponse<PartnerEnterPortalResult>> EnterPortal(PartnerEnterPortalCall call)
        {
            var response = await oWService.Post<PartnerEnterPortalResult>(endpoints.EnterPortal, call, OperatorId);

            if (response.Result.IsSuccessful())
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

            if (response.Result.IsSuccessful())
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

                if (call.Amount >= appSettings.HighStake)
                {
                    await adminApiSerivce.NotifyHighStake(new NotifyHighStakeRequest(
                        call.PlayerId,
                        call.GameTypeId,
                        call.Amount,
                        call.Currency,
                        call.ChoiceId,
                        system,
                        metadata,
                        call.OperatorId,
                        call.UserId));
                }
            }

            return response;
        }

        public async Task<PartnerResponse<PartnerEndGameResult>> EndGame(PartnerEndGameCall call)
        {
            var response = await oWService.Post<PartnerEndGameResult>(endpoints.EndGame, call, OperatorId);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(OperatorId, response);
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
    }
}