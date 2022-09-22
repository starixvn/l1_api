using L1.Features.AdminApiCommunicators;
using L1.Features.PartnerCommunicators;
using L1.Features.PartnerCommunicators.Maintenance;
using L1.Shared.Configurations;
using Sentry;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace L1.Features.SboCommunicators.CheckMaintenance
{
    public interface ISboMaintenanceService
    {
        Task ObserveMaintenanceSchedule(SboRequest request);
    }

    public class SboMaintenanceService : PartnerMaintenanceService, ISboMaintenanceService
    {
        private readonly SboServiceSettings sboServiceSettings;
        private readonly IAppSettings appSettings;

        public SboMaintenanceService(
            HttpClient httpClient,
            SboServiceSettings sboServiceSettings,
            IAdminApiService adminApiService,
            ISentryClient sentryClient,
            IAppSettings appSettings)
            : base(httpClient, adminApiService, sentryClient)
        {
            this.sboServiceSettings = sboServiceSettings;
            this.appSettings = appSettings;
        }

        public Task ObserveMaintenanceSchedule(SboRequest request)
        => ObserveMaintenanceSchedule(
                appSettings.PartnerIds.SBO,
                new PartnerRequest(request.Call),
                new Uri(sboServiceSettings.BaseUrl, sboServiceSettings.Endpoints.CheckMaintenance));
    }
}