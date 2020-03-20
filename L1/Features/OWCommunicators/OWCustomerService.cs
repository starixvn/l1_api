using System.Threading.Tasks;
using L1.Features.OWCommunicators.GetBalance;
using L1.Features.OWCommunicators.NotifyLogin;
using L1.Shared.Configurations;

namespace L1.Features.OWCommunicators
{

    public interface IOWCustomerService
    {
        Task<OWResponse<OWResult>> NotifyLogin(NotifyLoginCall call);

        Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call);
    }

    public class OWCustomerService : IOWCustomerService
    {

        private readonly IOWService oWService;
        private readonly IAppSettings appSettings;

        public OWCustomerService(IOWService oWService, IAppSettings appSettings)
        {
            this.oWService = oWService;
            this.appSettings = appSettings;
        }

        public async Task<OWResponse<OWResult>> NotifyLogin(NotifyLoginCall call)
        {
            return await oWService.Post<OWResult>(appSettings.OWServiceSettings.NotifyLoginEndPoint, call);
        }

        public async Task<OWResponse<GetBalanceResult>> GetBalance(GetBalanceCall call)
        {
            return await oWService.Post<GetBalanceResult>(appSettings.OWServiceSettings.GetBalanceEndPoint, call);
        }
    }
}
