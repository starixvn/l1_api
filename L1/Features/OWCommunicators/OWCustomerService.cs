

namespace L1.Features.OWCommunicators
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using L1.Shared.Configurations;

    public interface IOWCustomerService
    {
        Task<OWResponse<OWResult>> NotifyLogin(NotifyCustomerLoginCall call);
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

        public async Task<OWResponse<OWResult>> NotifyLogin(NotifyCustomerLoginCall call)
        {
            return await oWService.Post<OWResult>(appSettings.OWServiceSettings.NotifyLoginEndPoint, call);
        }
    }
}
