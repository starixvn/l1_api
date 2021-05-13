using System;
using System.Threading.Tasks;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.EnterPortal;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.GetBetLimit;
using L1.Features.OWCommunicators.HitJackpot;
using L1.Features.OWCommunicators.Log;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
using Polly;
using Sentry;
using L1.Shared.Constants;
using Sentry.Protocol;

namespace L1.Features.OWCommunicators
{
    public interface IOWCustomerService
    {
        Task<OWResponse<EnterPortalResult>> EnterPortal(EnterPortalCall call);

        Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call);

        Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call, byte system = Systems.Cocos);

        Task<OWResponse<EndGameResult>> EndGame(EndGameCall call);

        Task<OWResponse<VoidGameResult>> VoidGame(VoidGameCall call);

        Task<OWResponse<GetBetLimitResult>> GetBetLimit(GetBetLimitCall call);

        Task<OWResponse<HitJackpotResult>> HitJackpot(HitJackpotCall call);
    }

    public class OWCustomerService : IOWCustomerService
    {
        private readonly IOWService oWService;
        private readonly ILogService logService;
        private readonly OWEndPoints endpoints;

        public OWCustomerService(
            IOWService oWService,
            ILogService logService,
            OWEndPoints endpoints)
        {
            this.oWService = oWService;
            this.logService = logService;
            this.endpoints = endpoints;
        }

        public Task<OWResponse<EnterPortalResult>> EnterPortal(EnterPortalCall call)
            => oWService.Post<EnterPortalResult>(endpoints.EnterPortal, call);

        public Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call)
            => oWService.Post<GetBalanceResult>(endpoints.GetBalance, call);

        public async Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call, byte system = Systems.Cocos)
        {
            var response = await oWService.Post<PlaceBetResult>(endpoints.PlaceBet, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogBetAsync(response, system);
            }

            return response;
        }

        public async Task<OWResponse<EndGameResult>> EndGame(EndGameCall call)
        {
            var response = await oWService.Post<EndGameResult>(endpoints.EndGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogEndGameAsync(response);
            }

            return response;
        }

        public async Task<OWResponse<VoidGameResult>> VoidGame(VoidGameCall call)
        {
            var response = await oWService.Post<VoidGameResult>(endpoints.VoidGame, call);

            if (response.Result.IsSuccessful())
            {
                await logService.LogVoidGameAsync(response);
            }

            return response;
        }

        public async Task<OWResponse<HitJackpotResult>> HitJackpot(HitJackpotCall call)
        {
            const double retryDelayInSeconds = 30;
            const int numberOfRetries = 6;

            var response = await Policy
                .HandleResult<OWResponse<HitJackpotResult>>(response => response.Result.IsFailed())
                .WaitAndRetryAsync(numberOfRetries, retryAttempt => TimeSpan.FromSeconds(retryDelayInSeconds))
                .ExecuteAsync(() => oWService.Post<HitJackpotResult>(endpoints.HitJackpot, call));

            if (response.Result.IsSuccessful())
            {
                await logService.LogHitJackpotAsync(response);
            }
            else
            {
                SentrySdk.CaptureMessage($"Call HitJackpot failed for CustId {call.ObCustId}", SentryLevel.Fatal);
            }

            return response;
        }

        public async Task<OWResponse<GetBetLimitResult>> GetBetLimit(GetBetLimitCall call)
        {
            var response = await oWService.Post<GetBetLimitResult>(endpoints.GetBetLimit, call);

            return response;
        }
    }
}