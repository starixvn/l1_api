using System;
using System.Net.Http;
using System.Threading.Tasks;
using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Constants;
using Sentry;

namespace L1.Features.SboCommunicators.CheckMaintenance
{
    public interface ISboMaintenanceService
    {
        Task ObserveMaintenanceSchedule(SboRequest request);
    }

    public class SboMaintenanceService : PartnerMaintenanceService, ISboMaintenanceService
    {
        private readonly SboServiceSettings sboServiceSettings;

        public SboMaintenanceService(
            HttpClient httpClient,
            SboServiceSettings sboServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient)
            : base(httpClient, adminApiService, sentryClient)
        {
            this.sboServiceSettings = sboServiceSettings;
        }

        public Task ObserveMaintenanceSchedule(SboRequest request)
        => ObserveMaintenanceSchedule(
                PartnerIds.SBO,
                new PartnerRequest(request.Call),
                new Uri(sboServiceSettings.BaseUrl, sboServiceSettings.Endpoints.CheckMaintenance));
    }
}