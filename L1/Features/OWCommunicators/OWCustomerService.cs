using System.Threading.Tasks;
using L1.Features.OWCommunicators.EndGame;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.NotifyLogin;
using L1.Features.OWCommunicators.PlaceBet;
using L1.Features.OWCommunicators.VoidGame;
using L1.Shared.Configurations;

namespace L1.Features.OWCommunicators
{
    public interface IOWCustomerService
    {
        Task<OWResponse<NotifyResult>> NotifyLogin(NotifyLoginCall call);

        Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call);

        Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call);

        Task<OWResponse<EndGameResult>> EndGame(EndGameCall call);

        Task<OWResponse<VoidGameResult>> EndGame(VoidGameCall call);
    }

    public class OWCustomerService : IOWCustomerService
    {
        private readonly IOWService oWService;
        private readonly OWEndPoints endpoints;

        public OWCustomerService(IOWService oWService, IAppSettings appSettings)
        {
            this.oWService = oWService;
            endpoints = appSettings.OWServiceSettings.Endpoints;
        }

        public async Task<OWResponse<NotifyResult>> NotifyLogin(NotifyLoginCall call)
        {
            return await oWService.Post<NotifyResult>(endpoints.NotifyLogin, call);
        }

        public async Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call)
        {
            return await oWService.Post<GetBalanceResult>(endpoints.GetBalance, call);
        }

        public async Task<OWResponse<PlaceBetResult>> PlaceBet(PlaceBetCall call)
        {
            return await oWService.Post<PlaceBetResult>(endpoints.PlaceBet, call);
        }

        public async Task<OWResponse<EndGameResult>> EndGame(EndGameCall call)

        {
            return await oWService.Post<EndGameResult>(endpoints.EndGame, call);
        }

        public async Task<OWResponse<VoidGameResult>> EndGame(VoidGameCall call)

        {
            return await oWService.Post<VoidGameResult>(endpoints.VoidGame, call);
        }
    }
}